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
            return currentSession.Messages.Count > 0 &&
                timestamp < currentSession.Messages[currentSession.Messages.Count - 1].Timestamp;
        }

        /// <summary>
        /// At the end of this method, this class will be populated with 0 or more
        /// Ride objects.
        /// </summary>
        /// <param name="path"></param>
        public List<DeviceSession> Parse(string path)
        {
            // Open the file.
            foreach (var line in File.ReadLines(path))
            {
                Message message = Message.MessageFromLine(line);
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
                        currentSession.Messages.Add(message);
                }
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
