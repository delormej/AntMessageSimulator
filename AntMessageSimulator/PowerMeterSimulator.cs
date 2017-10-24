using System;
using System.IO;
using System.Collections.Generic;

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
            Console.WriteLine("ERROR! " + message);
            Console.WriteLine();
            PrintUsage();
        }

        private static void PrintInfo(string message)
        {
            Console.WriteLine(message);
        }

        private void GetSessionsFromFile()
        {
            DeviceLogParser parser = new DeviceLogParser();
            sessions = parser.Parse(source);

            if (sessions.Count == 0)
                throw new Exception("No sessions parsed from source: " + source);
        }

        private DeviceSession GetSingleSession()
        {
            if (sessionNumber == 0)
                return GetLastSession();
            else
                try
                {
                    return sessions[sessionNumber];
                }
                catch (IndexOutOfRangeException exception)
                {
                    throw new Exception("Invalid session number.", exception);
                }
        }

        DeviceSession GetLastSession()
        {
            if (sessions.Count == 0)
                throw new Exception("No sessions found.");

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
            foreach (var session in sessions)
                Console.WriteLine(session);
        }

        private void GenerateAutoAntsScript()
        {
            string script = "";
            DeviceSession session = GetSingleSession();

            using (AutoAntsScriptGenerator generator = new AutoAntsScriptGenerator(session))
            {
                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }

            PrintInfo("Writing output to file: " + destination);
            // Write the file out.
            File.WriteAllText(destination, script);
        }

        private void Execute()
        {
            GetSessionsFromFile();

            if (destination != null)
                GenerateAutoAntsScript();

            PrintSummary();
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
             *  - Was there an FE-C (rollers) device connected, if so - ID and channel
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
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }
    }
}

