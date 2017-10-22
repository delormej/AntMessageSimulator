using System;
using System.Collections.Generic;

/*
 * This program takes an ANT device log and simulates the power meter by opening an ANT+
 * channel and transmitting those messages.
 */
 namespace AntMessageSimulator
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }

        public MessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents a ride session as built from a device log.
    /// </summary>
    public class PowerMeterSession
    {
        public int DeviceId { get; }
        public int ChannelId { get; }
        public List<Message> Messages { get; }

    }

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
        static void ParseDeviceLog(string path)
        {
            // Open the file.

            // For each line, parse the message.
            // If timestamp < last timestamp, start a new session.
            // Check the channel id, 
            // If message is a set channel id, parse the device and channel ids.
            //  Once channel id is set, when a message is from the power meter, record that to the powerMeterSession object.


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
