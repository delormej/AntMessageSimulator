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
            Console.WriteLine(ExecutionOptions.GetUsage());
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
