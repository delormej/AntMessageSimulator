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
        /// <summary>
        /// At the end of this method, this class will be populated with 0 or more
        /// Ride objects.
        /// </summary>
        /// <param name="path"></param>
        public static List<DeviceSession> ParseDeviceLog(string path)
        {
            List<DeviceSession> sessions = new List<DeviceSession>();
            DeviceSession currentSession = null;

            // Open the file.
            foreach (var line in File.ReadLines(path))
            {
                Message message = Message.MessageFromLine(line);
                if (currentSession == null)
                {
                    currentSession = DeviceSession.GetDeviceSession(message);
                    if (currentSession != null)
                        sessions.Add(currentSession);
                }
                else
                {
                    if (currentSession.Messages.Count > 0 && message.Timestamp < 
                        currentSession.Messages[currentSession.Messages.Count - 1].Timestamp)
                    {
                        // Start a new session.
                        currentSession = null;
                    }
                    else
                    {
                        currentSession.Messages.Add(message);
                    }
                }                
            }

            return sessions;
        }

        static void PrintUsage()
        {
            const string USAGE =
                @"    Usage:    simulator.exe {Device Log} {Optional: Session Number} {Optional: .ants output script}
    Example:  simulator.exe ""C:\Program Files (x86)\Zwift\Device0.txt"" 1 Device0.ants
";
            Console.WriteLine(USAGE);
        }

        static void PrintWelcome()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Console.WriteLine("ANT+ Message Simulator version: " + version);
            Console.WriteLine();
        }

        static void PrintError(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            PrintUsage();
        }

        static void PrintWarning(string message)
        {
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            PrintWelcome();
            // A few different modes, if no session or output file is specified, just print the sessions.
            // If no output file is specified, default to the input filename with an .ants extension
            // 
            // Need option to actually invoke the ants script enginge (Future).


            // Validate arguments.
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            string source = args[1];
            string destination = args[2];
            string script = "";

            if (!File.Exists(source))
            {
                PrintError(string.Format("Source {0} does not exist!", source));
                return;
            }

            // Clean up previous run.
            if (File.Exists(destination))
            {
                File.Delete(destination);
                PrintWarning(string.Format("Overwriting previous file: {0}", destination));
            }

            // TODO: be able to list the # of sessions and the device Id.

            List<DeviceSession> sessions = ParseDeviceLog(source);

            // Use the last session to generate a script.
            using (AutoAntsScriptGenerator generator =
                new AutoAntsScriptGenerator(sessions[sessions.Count - 1]))
            {

                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }
            
            // Write the file out.
            File.WriteAllText(destination, script);
        }
    }
}

