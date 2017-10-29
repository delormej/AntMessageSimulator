using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AntMessageSimulator
{
    /// <summary>
    /// This helper class enacapsulates the state of iterating through the 
    /// appropriate sessions, respecting if simulator is run with a specific session #.
    /// </summary>
    internal class SelectedSessionsEnumerator : IEnumerator<DeviceSession>, IEnumerable<DeviceSession>
    {
        int index, count;
        bool awaitingFirstMove;
        List<DeviceSession> sessions;
        ExecutionOptions options;

        public SelectedSessionsEnumerator(List<DeviceSession> sessions, ExecutionOptions options)
        {
            this.sessions = sessions;
            this.options = options;
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
            if (options.SessionNumber > 0)
            {
                index = options.SessionNumber - 1;
                count = 1;

                if (index >= sessions.Count())
                    throw new ApplicationException("Invalid session number: " +
                        options.SessionNumber);
            }
            else
            {
                index = 0;
                count = sessions.Count();
            }

            awaitingFirstMove = true;
        }

        DeviceSession IEnumerator<DeviceSession>.Current => sessions[index];
        public object Current => sessions[index];
        public IEnumerator<DeviceSession> GetEnumerator() { return this; }       
        IEnumerator IEnumerable.GetEnumerator() { return this; }
        public void Dispose() { }
    }
}
