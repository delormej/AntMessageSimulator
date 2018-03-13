using System;
using AntMessageSimulator.Messages;

namespace AntMessageSimulator
{
    public class MessageFactory
    {
        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            return MessageFromLine(DeviceType.Unassigned, line);
        }

        public static Message MessageFromLine(DeviceType deviceType, string line)
        {
            Message message = new Message(line);
            return CreateMessageTypeInstance(deviceType, message);
        }

        public static Message MessageFromLine(DeviceSession session, string line)
        {
            DeviceType deviceType = DeviceType.Unassigned;
            Message message = new Message(line);
            if (message.ChannelId == session.FecChannelId)
                deviceType = DeviceType.FeC;
            else if (message.ChannelId == session.PowerMeterChannelId)
                deviceType = DeviceType.PowerMeter;

            return CreateMessageTypeInstance(deviceType, message);
        }

        private static Message CreateMessageTypeInstance(DeviceType deviceType, Message message)
        {
            Type t = MessageType.GetType(deviceType, message.MessageId);
            if (t != null)
                return Activator.CreateInstance(t, message) as Message;
            else
                return message;
        }
    }
}
