using System;
using System.IO;
using System.Collections.Generic;

/*
 * TODO: 
 *  - Invoke AutoANTS libraries to play back the .ants script that send ANT+ messages on the wire.
 *  - Add chart option that uploads json file to Azure blob, launches chart.html with path as param
 *  - Invoke PCLAB2000 libraries to execute function generator for speed simulation
  */
namespace AntMessageSimulator
{
    public class Simulator
    {
        private ExecutionOptions options;
        private List<DeviceSession> sessions;

        public Simulator(ExecutionOptions options)
        {
            this.options = options;
        }

        public void Execute()
        {
            GetSessionsFromFile();
            PrintSummary();
            if (options.Operation > OperationType.SummaryOnly)
                GenerateAndWriteOutput();
        }

        private void GetSessionsFromFile()
        {
            DeviceLogParser parser = new DeviceLogParser();
            sessions = parser.Parse(options.Source);
            if (sessions.Count == 0)
                throw new ApplicationException("No sessions parsed from source: " + options.Source);
        }

        private void PrintSummary()
        {
            Printer.Info(string.Format("File contained {0} session(s).", sessions.Count));
            var enumerator = GetSelectedSessions();
            foreach (var session in enumerator)
                PrintSessionSummary(session, enumerator.Index);
        }

        private void PrintSessionSummary(DeviceSession session, int index)
        {
            Printer.Info(string.Format("\t{0}: {1}", index + 1, session));
        }

        private void GenerateAndWriteOutput()
        {
            var enumerator = GetSelectedSessions();
            foreach (var session in enumerator)
            {
                string content = Generate(session);
                WriteOutput(content, enumerator.Index, enumerator.Count);
            }
        }

        private SelectedSessionsEnumerator GetSelectedSessions()
        {
            return new SelectedSessionsEnumerator(sessions, options);
        }

        private string Generate(DeviceSession session)
        {
            Generator generator = GeneratorFactory.Create(session, options);
            return generator.Generate();
        }

        private void WriteOutput(string content, int sessionIndex, int sessionCount)
        {
            if (content == string.Empty)
                throw new ApplicationException("Nothing to write for session: " + sessionIndex + 1);

            if (options.Output == OutputType.Console)
                Printer.Info(content);
            else if (options.Output == OutputType.File)
            {
                string filename = options.GetDestinationFilename(sessionIndex, sessionCount);
                WriteFile(filename, content);
            }
        }

        private void WriteFile(string filename, string content)
        {
            Printer.Info("Writing output to file: " + filename);
            File.WriteAllText(filename, content);
            if (options.CloudUpload)
                CloudUpload(filename);
        }

        private void CloudUpload(string filename)
        {
            Printer.Info(string.Format("Uploading {0} to cloud.", Path.GetFileName(filename)));
            CloudStorage storage = new CloudStorage();
            storage.Upload(filename);
        }
    }
}

