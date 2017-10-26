using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

/*
 * This program takes an ANT device log and simulates the power meter by opening an ANT+
 * channel and transmitting those messages.
 */
namespace AntMessageSimulator
{
    // The act of parsing the log should implement the visitor pattern, such that other
    // operations, i.e. speed events or FE-C errors could be gleaned from this log.

    public class PowerMeterSimulator
    {
        private string source;
        private string destination;
        private string[] args;
        private int sessionNumber;
        private bool printFec;
        private List<DeviceSession> sessions;

        private static void PrintWelcome()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Console.WriteLine("ANT+ Message Simulator version: " + version);
            Console.WriteLine();
        }

        private static void PrintUsage()
        {
            const string USAGE =
                @"    Usage:    simulator.exe {Device Log} {Optional: Session Number} {Optional: .ants output script}
    Example:  simulator.exe ""C:\Program Files (x86)\Zwift\Device0.txt"" 1 Device0.ants
";
            Console.WriteLine(USAGE);
            Console.WriteLine();
        }

        private static void PrintError(string message)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR! " + message);
            Console.ForegroundColor = defaultColor;
        }

        private static void PrintInfo(string message)
        {
            /*
             * TODO: This logic SHOULD NOT be in here... temporary hack.
             */
            if (message.Contains("ServoPosition = 800"))
                PrintError(message);
            else
                Console.WriteLine(message);
        }

        private void GetSessionsFromFile()
        {
            DeviceLogParser parser = new DeviceLogParser();
            sessions = parser.Parse(source);

            if (sessions.Count == 0)
                throw new ApplicationException("No sessions parsed from source: " + source);
        }

        private DeviceSession GetSession()
        {
            if (sessions.Count < sessionNumber)
                throw new ApplicationException("Invalid session number.");

            return sessions[sessionNumber - 1];
        }

        DeviceSession GetLastSession()
        {
            if (sessions.Count == 0)
                throw new ApplicationException("No sessions found.");

            return sessions[sessions.Count - 1];
        }

        private void ValidateSource(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing source file.", path);
            else
                source = path;
        }

        private void ValidateDestination(string value)
        {
            // Should be the destination filename.
            destination = value;

            // Clean up previous run.
            DeleteDestination();
        }

        private void ValidateDestinationOrSession(string value)
        {
            if (!int.TryParse(value, out sessionNumber))
                ValidateDestination(value);
        }

        private void DeleteDestination()
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
                PrintInfo(string.Format("Overwriting previous file: {0}", destination));
            }
        }

        private void ParseArgs()
        {
            if (args.Length == 0)
                throw new ArgumentException("Invalid number of parameters.");
            if (args.Length > 0)
                ValidateSource(args[0]);
            if (args.Length > 1)
                ValidateDestinationOrSession(args[1]);
            if (args.Length > 2)
                ValidateDestination(args[2]);
        }

        private void PrintSummary()
        {
            Console.WriteLine(string.Format("File contained {0} session(s).", sessions.Count));
            Console.WriteLine("Working on these sessions:");

            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
                Console.WriteLine("\t{0}: {1}", enumerator.Index + 1, session);
        }

        private void PrintFecCommands(DeviceSession session)
        {
            PowerMeterEventsQuery query = new PowerMeterEventsQuery(session);
            var events = query.FindAllFecEvents();
            foreach (var info in events)
                if (info != null)
                    PrintInfo(info.ToString());
        }

        private void PrintAllFecCommands()
        {
            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
            {
                if (session.FecId > 0)
                    PrintFecCommands(session);
            }
        }

        private string GenerateAutoAntsScript(DeviceSession session)
        {
            string script = "";

            using (AutoAntsScriptGenerator generator = new AutoAntsScriptGenerator(session))
            {
                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }

            return script;
        }

        private string AppendToDestinationFilename(string value)
        {
            string name = Path.GetFileNameWithoutExtension(destination) + "-" + value;
            string newDestination = destination.Replace(Path.GetFileName(destination),
                name + Path.GetExtension(destination));

            return newDestination;
        }

        private string GetDestinationFilename(int index, int count)
        {
            string filename;
            if (count > 1)
                filename = AppendToDestinationFilename(index.ToString());
            else
                filename = destination;

            return filename;
        }

        private void WriteAutoAntsFiles()
        {
            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
            {
                string filename = GetDestinationFilename(enumerator.Index, enumerator.Count);
                string script = GenerateAutoAntsScript(session);

                PrintInfo("Writing output to file: " + filename);
                File.WriteAllText(filename, script);
            }
        }

        private void Execute()
        {
            GetSessionsFromFile();

            if (destination != null)
                WriteAutoAntsFiles();

            PrintSummary();

            if (printFec)
                PrintAllFecCommands();
        }

        /// <summary>
        /// Creates a new simulator with a variable list of arguments from the command line.
        /// </summary>
        /// <param name="args"></param>
        public PowerMeterSimulator(string[] args)
        {
            this.args = args;
            ParseArgs();
        }

        static void Main(string[] args)
        {
            /*
             * TODO: This program needs to answer a couple of additional questions:
             *  - What was the total duration of the session
             *  - Generate seperate file for times and speed (CSV?) as reported by rollers
             *  - Include commands sent FROM Zwift/app to set resistance in the .ants script
             */

            PrintWelcome();
            try
            {
                PowerMeterSimulator simulator = new PowerMeterSimulator(args);
                simulator.Execute();
            }
            catch (ApplicationException e)
            {
                PrintError(e.Message);
                PrintUsage();
            }
        }

        /// <summary>
        /// This nestd helper class enacapsulates the state of iterating through the 
        /// appropriate sessions.
        /// </summary>
        private class SessionEnumerator : IEnumerator<AntMessageSimulator.DeviceSession>, IEnumerable<DeviceSession>
        {
            int index, count;
            bool awaitingFirstMove;
            PowerMeterSimulator parent;

            public SessionEnumerator(PowerMeterSimulator parent)
            {
                this.parent = parent;
                Reset();
            }

            public int Count { get { return count; } }
            public int Index { get { return index; } }

            public bool MoveNext()
            {
                if (awaitingFirstMove)
                {
                    awaitingFirstMove = false;
                    return true;
                }
                if (++index < count)
                    return true;
                else
                    return false;
            }

            public void Reset()
            {
                if (parent.sessionNumber > 0)
                {
                    index = parent.sessionNumber - 1;
                    count = 1;

                    if (index >= parent.sessions.Count)
                        throw new ApplicationException("Invalid session number: " + parent.sessionNumber);
                }
                else
                {
                    index = 0;
                    count = parent.sessions.Count;
                }

                awaitingFirstMove = true;
            }

            DeviceSession IEnumerator<DeviceSession>.Current => parent.sessions[index];
            object IEnumerator.Current => throw new NotImplementedException();
            public IEnumerator<DeviceSession> GetEnumerator() { return this; }
            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
            public void Dispose() { }
        }
    }
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

