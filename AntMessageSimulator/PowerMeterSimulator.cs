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

        private static void PrintWarning(string message)
        {
            Console.WriteLine(message);
        }

        private void GetSessionsFromFile(string source)
        {
            DeviceLogParser parser = new DeviceLogParser();
            sessions = parser.Parse(source);
        }

        DeviceSession GetLastSession()
        {
            return sessions[sessions.Count - 1];
        }

        private void ValidateSource(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Missing source file.", path);
            }
            else
            {
                source = path;
            }
        }

        private void ValidateDestinationOrSession(string value)
        {
            if (!int.TryParse(value, out sessionNumber))
            {
                // Should be the destination filename.
                destination = value;

                // Clean up previous run.
                DeleteDestination();
            }
        }

        private void DeleteDestination()
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
                PrintWarning(string.Format("Overwriting previous file: {0}", destination));
            }
        }

        private void ParseArgs()
        {
            // Validate arguments.
            switch (args.Length)
            { 
                case 0:
                    throw new ArgumentException("Invalid number of parameters.");
                case 1:
                    ValidateSource(args[0]);
                    break;
                case 2:
                    ValidateDestinationOrSession(args[1]);
                    break;
                default:
                    break;
            }
        }

        private void PrintSummary()
        {
            Console.WriteLine(string.Format("File contained {0} session(s).", sessions.Count));
            foreach (var session in sessions)
                Console.WriteLine(session);
        }

        private void Execute()
        {
            string script = "";

            GetSessionsFromFile(source);
            DeviceSession session = GetLastSession();

            if (destination == null)
            {
                PrintSummary();
                return;
            }

            using (AutoAntsScriptGenerator generator = new AutoAntsScriptGenerator(session))
            {
                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }

            // Write the file out.
            File.WriteAllText(destination, script);
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

