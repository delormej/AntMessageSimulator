using System;
using System.IO;

namespace AntMessageSimulator
{
    public enum DeviceType
    {
        Unassigned = 0,
        PowerMeter,    
        FeC
    }

    public class ExecutionOptions
    {
        private string source;
        private string destination;
        private string[] args;
        private int sessionNumber;
        // TODO: this should now be an exclusive enum because cannot execute more than one of these at a time.
        private bool outputAnts;
        private bool outputJson;
        private bool outputSpeed;
        private bool outputConsole;
        private DeviceType device;
        private ushort targetDeviceId;

        public static string GetUsage()
        {
            const string USAGE =
                @"    Usage:    simulator.exe {Device Log} {Optional: Session Number} {output filename} {Optional: --ants | --fec | --json | --cout}
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
            set { destination = value; }
        }

        public int SessionNumber
        {
            get { return sessionNumber; }
            set { sessionNumber = value; }
        }

        public bool OutputAnts
        {
            get { return outputAnts; }
            set { outputAnts = value; }
        }

        public bool OutputJson
        {
            get { return outputJson;  }
            set { outputJson = value; }
        }

        public bool OutputSpeed
        {
            get { return outputSpeed; }
            set { outputSpeed = value; }
        }

        public bool OutputConsole
        {
            get { return outputConsole; }
            set { outputConsole = value; }
        }

        public DeviceType Device
        {
            get { return device;  }
            set { device = value; }
        }

        public bool WriteOutput()
        {
            return (destination != null && (outputAnts || outputJson || outputSpeed)) ||
                outputConsole;
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
            ValidateOutputOptions();
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

            // Should be the destination filename.
            destination = value;

            // Clean up previous run.
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
                SetDeviceType(DeviceType.FeC);
            else if (value.ToUpper() == "BP")
                SetDeviceType(DeviceType.PowerMeter);
            else if (value.ToUpper() == "JSON")
                outputJson = true;
            else if (value.ToUpper() == "ANTS")
                outputAnts = true;
            else if (value.ToUpper() == "HZ")
                outputSpeed = true;
            else if (value.ToUpper() == "COUT")
                outputConsole = true;
            else
                throw new ApplicationException(value + " is not a valid option.");
        }

        private void SetDeviceType(DeviceType deviceType)
        {
            if (device == DeviceType.Unassigned)
                device = deviceType;
            else
                throw new ApplicationException(
                    "You must choose --bp (Bike Power) OR --fec (FE-C Trainer), not both.");
        }

        private void ValidateDeviceOrSetDefault(DeviceType deviceType)
        {
            if (device == DeviceType.Unassigned)
                device = deviceType;
        }

        private void ValidateOutputOptions()
        {
            if (outputAnts == false && outputJson == false && destination != null)
                SetImplicitOutputType();
            else if (outputAnts == true && outputJson == true)
                throw new ApplicationException("You must choose --ants OR --json as output, not both.");
            else if ((outputAnts == true || outputJson == true) && destination == null)
                throw new ApplicationException("You must specify a destination file.");
        }

        /// <summary>
        /// If the user has not specified --ants or --json, implicitly derive this from the 
        /// extension of the destination parameter.
        /// </summary>
        private void SetImplicitOutputType()
        {
            string extension = Path.GetExtension(destination);
            if (extension == ".ants")
                outputAnts = true;
            else if (extension == ".json")
                outputJson = true;
        }
    }
}
