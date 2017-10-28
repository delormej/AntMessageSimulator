using System;

namespace AntMessageSimulator
{
    public class Printer
    {
        public static void Welcome()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Console.WriteLine("ANT+ Message Simulator version: " + version);
            Console.WriteLine();
        }

        public static void Usage()
        {
            const string USAGE =
                @"    Usage:    simulator.exe {Device Log} {Optional: Session Number} {output filename} {Optional: --ants | --fec | --json}
    Example:  simulator.exe Device0.txt                     #Lists a session summary for each in the device log.
    Example:  simulator.exe Device0.txt 1 Device0.ants      #Outputs an AutoANTs .ants script file generated from session #1.
    Example:  simulator.exe Device0.txt 2 --fec             #Prints all FEC commands to console from the second session in the device log.
";
            Console.WriteLine(USAGE);
            Console.WriteLine();
        }

        public static void Info(string message)
        {
            /*
             * TODO: This logic SHOULD NOT be in here... temporary hack.
             */
            if (message.Contains("ServoPosition\":800"))
                Error(message);
            else
                Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR! " + message);
            Console.ForegroundColor = defaultColor;
        }
    }
}
