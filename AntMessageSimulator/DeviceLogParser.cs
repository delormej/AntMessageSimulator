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
            Message lastMessage = currentSession.GetLastMessage();

            if (lastMessage != null)
                return timestamp < lastMessage.Timestamp;
            else
                return false;
        }

        private void AddSession()
        {
            bool isValidSession = (currentSession.PowerMeterId > 0);

            // Don't save invalid sessions.
            if (!isValidSession)
                sessions.Remove(currentSession);

            currentSession = new DeviceSession();
            sessions.Add(currentSession);
        }

        private void AddMessageToSession(Message message)
        {
            if (IsTimestampRollover(message.Timestamp))
                AddSession();
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
            currentSession = new DeviceSession();
            sessions = new List<DeviceSession>();
            sessions.Add(currentSession);
        }
    }
}
