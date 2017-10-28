using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

/*
 * TODO: This program needs to answer a couple of additional questions:
 *  - What was the total duration of the session
 *  - Generate seperate file for times and speed (CSV?) as reported by rollers
 *  - Include commands sent FROM Zwift/app to set resistance in the .ants script
 */
namespace AntMessageSimulator
{
    public class Simulator
    {
        private ExecutionOptions options;
        private List<DeviceSession> sessions;

        static void Main(string[] args)
        {
            Printer.Welcome();
            try
            {
                Simulator simulator = new Simulator(args);
                simulator.Execute();
            }
            catch (ApplicationException e)
            {
                Printer.Error(e.Message);
                Printer.Usage();
            }
        }

        /// <summary>
        /// Creates a new simulator with a variable list of arguments from the command line.
        /// </summary>
        /// <param name="args"></param>
        public Simulator(string[] args)
        {
            options = new ExecutionOptions(args);
        }

        private void Execute()
        {
            GetSessionsFromFile();
            PrintSummary();

            if (options.WriteOutput())
                WriteOutput();
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
                Printer.Info(string.Format("\t{0}: {1}", enumerator.Index + 1, session));
        }

        private void WriteOutput()
        {
            SessionEnumerator enumerator = new SessionEnumerator(this);
            foreach (var session in enumerator)
            {
                string filename = options.GetDestinationFilename(enumerator.Index, enumerator.Count);
                string content = null;

                if (options.OutputAnts)
                    content = GenerateAutoAntsScript(session);
                else if (options.OutputJson)
                    content = GenerateJson(session);

                if (content != null)
                    WriteFile(filename, content);
                else
                    throw new ApplicationException(string.Format("Unable to generate output for session: {0}",
                        enumerator.Index + 1));
            }
        }

        private void WriteFile(string filename, string content)
        {
            Printer.Info("Writing output to file: " + filename);
            File.WriteAllText(filename, content);
        }

        private string GenerateAutoAntsScript(DeviceSession session)
        {
            string script = "";
            using (AutoAntsScriptGenerator generator = 
                new AutoAntsScriptGenerator(session, options.Device))
            {
                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);
                script = reader.ReadToEnd();
            }
            return script;
        }

        private string GenerateJson(DeviceSession session)
        {
            MessageQuery query = new MessageQuery(session);
            var events = query.FindAllFecEvents();
            return JsonConvert.SerializeObject(events);
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

