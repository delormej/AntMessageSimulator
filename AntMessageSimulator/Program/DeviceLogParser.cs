using System;
using System.IO;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class DeviceLogParser
    {
        List<DeviceSession> sessions;
        DeviceSession currentSession;

        private bool IsTimestampRollover(float timestamp)
        {
            return timestamp < currentSession.GetLastTimestamp();
        }

        private void AddNewSession()
        {
            // Don't save invalid sessions.
            if (!currentSession.IsValid())
                sessions.Remove(currentSession);

            currentSession = new DeviceSession();
            sessions.Add(currentSession);
        }

        private void AddMessageToSession(Message message)
        {
            if (IsTimestampRollover(message.Timestamp))
                AddNewSession();
            else
                currentSession.AddMessage(message);
        }

        /// <summary>
        /// At the end of this method, this class will be populated with 0 or more DeviceSession objects.
        /// </summary>
        /// <param name="path"></param>
        public List<DeviceSession> Parse(string path)
        {
            // Open the file.
            foreach (var line in File.ReadLines(path))
            {
                Message message = Message.MessageFromLine(line);
                AddMessageToSession(message);
            }

            return sessions;
        }

        public DeviceLogParser()
        {
            sessions = new List<DeviceSession>();
            currentSession = new DeviceSession();
            sessions.Add(currentSession);
        }
    }
}
