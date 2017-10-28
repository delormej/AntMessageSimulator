using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

/*
 * TODO: 
 *  - Generate seperate file for speed to play back with PCLAB2000 Function Generator.
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
                Generator generator = CreateGenerator(session);
                string content = generator.Generate();

                if (options.WriteOutput() && content != null)
                {
                    string filename = options.GetDestinationFilename(enumerator.Index, enumerator.Count);
                    WriteFile(filename, content);
                }
            }
        }

        private Generator CreateGenerator(DeviceSession session)
        {
            Generator generator = null;

            if (options.OutputAnts)
                generator = new AutoAntsScriptGenerator(session, options.Device);
            else if (options.OutputJson)
                generator = new JsonGenerator(session);
            else if (options.OutputSpeed)
                generator = new WaveGenerator(session);

            return generator;
        }

        private void WriteFile(string filename, string content)
        {
            Printer.Info("Writing output to file: " + filename);
            File.WriteAllText(filename, content);
        }
                
        /// <summary>
        /// This nestd helper class enacapsulates the state of iterating through the 
        /// appropriate sessions.
        /// </summary>
        private class SessionEnumerator : IEnumerator<AntMessageSimulator.DeviceSession>, IEnumerable<DeviceSession>
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
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

