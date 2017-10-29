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
            if (options.WriteOutput())
                GenerateAndWriteOutput();
        }

        private void GetSessionsFromFile()
        {
            DeviceLogParser parser = new DeviceLogParser();
            sessions = parser.Parse(options.Source);
            if (sessions.Count == 0)
                throw new ApplicationException("No sessions parsed from source: " +
                    options.Source);
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
            Generator generator = CreateGenerator(session);
            return generator.Generate();
        }

        private Generator CreateGenerator(DeviceSession session)
        {
            Generator generator = null;
            if (options.OutputAnts)
                generator = new AutoAntsScriptGenerator(session, options.Device);
            else if (options.OutputJson)
                generator = new JsonGenerator(session, options.Device);
            else if (options.OutputSpeed)
                generator = new WaveGenerator(session);
            return generator;
        }

        private void WriteOutput(string content, int sessionIndex, int sessionCount)
        {
            if (content == null)
                throw new ApplicationException("Nothing to write for session: " + sessionIndex + 1);

            string filename = options.GetDestinationFilename(sessionIndex, sessionCount);
            WriteFile(filename, content);
        }

        private void WriteFile(string filename, string content)
        {
            Printer.Info("Writing output to file: " + filename);
            File.WriteAllText(filename, content);
        }
    }
}

