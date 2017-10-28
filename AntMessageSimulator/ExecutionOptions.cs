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

    internal class ExecutionOptions
    {
        private string source;
        private string destination;
        private string[] args;
        private int sessionNumber;
        private bool outputAnts;
        private bool outputJson;
        private DeviceType device;

        public string Source
        {
            get { return source;  }
        }

        public string Destination
        {
            get { return destination; }
        }

        public int SessionNumber
        {
            get { return sessionNumber; }
        }

        public bool OutputAnts
        {
            get { return outputAnts; }
        }

        public bool OutputJson
        {
            get { return outputJson;  }
        }

        public DeviceType Device
        {
            get { return device;  }
        }

        public bool WriteOutput()
        {
            return destination != null && (outputAnts || OutputJson);
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
