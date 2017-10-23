using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

/*
 * This program takes an ANT device log and simulates the power meter by opening an ANT+
 * channel and transmitting those messages.
 */
namespace AntMessageSimulator
{
    // The act of parsing the log should implement the visitor pattern, such that other
    // operations, i.e. speed events or FE-C errors could be gleaned from this log.

    public class Program
    {
        private List<PowerMeterSession> powerMeterSessions;

        /// <summary>
        /// At the end of this method, this class will be populated with 0 or more
        /// Ride objects.
        /// </summary>
        /// <param name="path"></param>
        public static List<PowerMeterSession> ParseDeviceLog(string path)
        {
            List<PowerMeterSession> sessions = new List<PowerMeterSession>();
            PowerMeterSession currentSession = null;

            // Open the file.
            foreach (var line in File.ReadLines(path))
            {
                Message message = Message.MessageFromLine(line);
                if (currentSession == null)
                {
                    currentSession = PowerMeterSession.GetPowerMeterSession(message);
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

            // For each line, parse the message.
            // If timestamp < last timestamp, start a new session.
            // Check the channel id, 
            // If message is a set channel id, parse the device and channel ids.
            //  Once channel id is set, when a message is from the power meter, record that to the powerMeterSession object.

            return sessions;
        }

        static void Main(string[] args)
        {
            // Read a Device*.txt|log file, line by line.

            // Pre-parse
            // Identify & break file into multiple streams if the timestamp resets.
            // Identify if there are power meter messages in the stream.
            // Per stream, identify all connected devices (channel Id, device type, device id)



            // Extract all relevant events for one or more channels.
            // For all Rx events on a given channel.
            // Truncate extended messages at 8 bytes.

            // Create an .ants file that:
            // 1. Assigns, configures and opens the appropriate channel for a given (channelId, deviceType, deviceId)
            // 

            // n. Close the channel.

            // Execute the script, logging and exiting as necessary.

        }
    }
}

