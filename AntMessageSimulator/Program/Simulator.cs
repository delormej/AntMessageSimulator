using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

/*
 * TODO: 
 *  - Add chart option that uploads json file to Azure blob, launches chart.html with path as param
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
            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
                PrintSessionSummary(session, enumerator.Index);
        }

        private void PrintSessionSummary(DeviceSession session, int index)
        {
            Printer.Info(string.Format("\t{0}: {1}", index + 1, session));
        }

        private void GenerateAndWriteOutput()
        {
            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
            {
                string content = Generate(session);
                WriteOutput(content, enumerator.Index, enumerator.Count);
            }
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
                
        /// <summary>
        /// This nestd helper class enacapsulates the state of iterating through the 
        /// appropriate sessions, respecting if simulator is run with a specific session #.
        /// </summary>
        private class SessionEnumerator : IEnumerator<DeviceSession>, IEnumerable<DeviceSession>
        {
            int index, count;
            bool awaitingFirstMove;
            Simulator parent;

            public SessionEnumerator(Simulator parent)
            {
                this.parent = parent;
                Reset();
            }

            public int Count
            {
                get { return count; }
            }

            public int Index
            {
                get { return index; }
            }

            public bool MoveNext()
            {
                if (awaitingFirstMove)
                {
                    awaitingFirstMove = false;
                    return true;
                }
                if (++index < count)
                    return true;
                else
                    return false;
            }

            public void Reset()
            {
                if (parent.options.SessionNumber > 0)
                {
                    index = parent.options.SessionNumber - 1;
                    count = 1;

                    if (index >= parent.sessions.Count)
                        throw new ApplicationException("Invalid session number: " + 
                            parent.options.SessionNumber);
                }
                else
                {
                    index = 0;
                    count = parent.sessions.Count;
                }

                awaitingFirstMove = true;
            }

            DeviceSession IEnumerator<DeviceSession>.Current => parent.sessions[index];
            object IEnumerator.Current => throw new NotImplementedException();
            public IEnumerator<DeviceSession> GetEnumerator() { return this; }
            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
            public void Dispose() { }
        }
    }
}

