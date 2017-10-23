using System;
using System.IO;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class DeviceLogParser
    {
        /// <summary>
        /// At the end of this method, this class will be populated with 0 or more
        /// Ride objects.
        /// </summary>
        /// <param name="path"></param>
        public static List<DeviceSession> Parse(string path)
        {
            List<DeviceSession> sessions = new List<DeviceSession>();
            DeviceSession currentSession = null;

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
                    if (currentSession.Messages.Count > 0 && message.Timestamp <
                        currentSession.Messages[currentSession.Messages.Count - 1].Timestamp)
                    {
                        // Start a new session.
                        currentSession = null;
                    }
                    else
                    {
                        currentSession.Messages.Add(message);
                    }
                }
            }

            return sessions;
        }
    }
}
