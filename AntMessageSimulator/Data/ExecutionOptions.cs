using System;
using System.IO;

namespace AntMessageSimulator
{
    public class ExecutionOptions
    {
        private string[] args;
        private string source;
        private string destination;
        private int sessionNumber;
        private DeviceType device;
        private OperationType operation;

        public static string GetUsage()
        {
            const string USAGE =
                @"    Usage:    simulator.exe {Device Log} {Optional: Session Number} {output filename} {Optional: --ants | --fec | --json | --cout | --c}
    Example:  simulator.exe Device0.txt                     #Lists a session summary for each in the device log.
    Example:  simulator.exe Device0.txt 1 Device0.ants      #Outputs an AutoANTs .ants script file generated from session #1.
    Example:  simulator.exe Device0.txt 2 --fec --cout      #Prints all FEC commands from session 2 to console.
";
            return USAGE;
        }

        public string Source
        {
            get { return source;  }
            set { source = value; }
        }

        public string Destination
        {
            get { return destination; }
            set
            {
                if (!string.IsNullOrEmpty(value)
                        && Path.GetFileName(value).IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                    throw new ApplicationException("Invalid destination filename passed: " + value);
                destination = value;
                Output = OutputType.File;
            }
        }

        public int SessionNumber
        {
            get { return sessionNumber; }
            set { sessionNumber = value; }
        }

        public OperationType Operation {
            get { return operation; }
            set
            {
                if (operation != OperationType.Unassigned)
                    throw new ApplicationException("You cannot choose more than one operation.");
                else
                    operation = value;
            }
        }

        public OutputType Output { get; set; }

        public DeviceType Device
        {
            get { return device;  }
            set
            {
                if (device == DeviceType.Unassigned)
                    device = value;
                else
                    throw new ApplicationException(
                        "You must choose --bp (Bike Power) OR --fec (FE-C Trainer), not both.");
            }
        }

        public bool WriteToFile
        {
            get { return destination != null; }
        }

        public ExecutionOptions(string[] args)
        {
            this.args = args;
            ParseArgs();
        }

        public string GetDestinationFilename(int index, int count)
        {
            string filename;
            if (count > 1)
                filename = AppendToDestinationFilename(index.ToString());
            else
                filename = destination;

            return filename;
        }

        private string AppendToDestinationFilename(string value)
        {
            string name = Path.GetFileNameWithoutExtension(destination) + "-" + value;
            string newDestination = destination.Replace(Path.GetFileName(destination),
                name + Path.GetExtension(destination));

            return newDestination;
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

            ParseOptions();
            ValidateDeviceOrSetDefault(DeviceType.PowerMeter);
            ValidateOperationOrSetDefault(OperationType.SummaryOnly);
        }

        private void ValidateSource(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing source file.", path);
            else
                source = path;
        }

        private void ValidateDestinationOrSession(string value)
        {
            if (ParseArgAsOption(value))
                return;

            if (!int.TryParse(value, out sessionNumber))
                ValidateDestination(value);
        }

        private void ValidateDestination(string value)
        {
            if (ParseArgAsOption(value))
                return;
            Destination = value;
            DeleteDestination();
        }

        private void DeleteDestination()
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
                Printer.Info(string.Format("Overwriting previous file: {0}", destination));
            }
        }

        private void ParseOptions()
        {
            for (int i = 3; i < args.Length; i++)
                if (args[i].StartsWith("--"))
                    ParseArgOption(args[i].Substring(2, args[i].Length - 2));
        }

        private bool ParseArgAsOption(string value)
        {
            bool isOption = value.StartsWith("--");

            if (isOption)
                ParseArgOption(value.Substring(2, value.Length - 2));

            return isOption;
        }

        private void ParseArgOption(string value)
        {
            if (value.ToUpper() == "FEC")
                Device = DeviceType.FeC;
            else if (value.ToUpper() == "BP")
                Device = DeviceType.PowerMeter;
            else if (value.ToUpper() == "JSON")
                Operation = OperationType.Json;
            else if (value.ToUpper() == "ANTS")
                Operation = OperationType.Ants;
            else if (value.ToUpper() == "HZ")
                Operation = OperationType.Hz;
            else if (value.ToUpper() == "C")
                Operation = OperationType.CArray;
            else if (value.ToUpper() == "COUT")
                Output = OutputType.Console;

            else
                throw new ApplicationException(value + " is not a valid option.");
        }
        
        private void ValidateDeviceOrSetDefault(DeviceType deviceType)
        {
            if (device == DeviceType.Unassigned)
                device = deviceType;
        }

        /// <summary>
        /// If the user has not specified --ants or --json, implicitly derive this from the 
        /// extension of the destination parameter.
        /// </summary>
        private void ValidateOperationOrSetDefault(OperationType defaultOperation)
        {
            if (destination != null)
            {
                string extension = Path.GetExtension(destination);
                if (extension == ".ants")
                    Operation = OperationType.Ants;
                else if (extension == ".json")
                    Operation = OperationType.Json;
            }
            else
                Operation = defaultOperation;
        }
    }
}
