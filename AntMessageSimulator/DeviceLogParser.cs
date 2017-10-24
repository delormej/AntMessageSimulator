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

        private void AddMessageToSession(Message message)
        {
            /* 
             * A few things can happen here:
             * 1) A new session gets generated.
             * 2) A session can glean some state; device id, channel id, etc...
             * 3) Needs to be extensible such that we can define new an interesting things we want from these messages
             *      without changing this code.
             */
            if (currentSession == null)
            {
                currentSession = DeviceSession.GetDeviceSession(message);
                if (currentSession != null)
                    sessions.Add(currentSession);
            }
            else
            {
                if (IsTimestampRollover(message.Timestamp))
                    // Start a new session.
                    currentSession = null;
                else
                    // Add message to the session.
                    currentSession.AddMessage(message);
            }
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
            currentSession = null;
        }
    }
}
