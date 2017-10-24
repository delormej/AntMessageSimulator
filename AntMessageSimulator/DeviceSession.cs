using System;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    /// <summary>
    /// Represents a ride session as built from a device log.
    /// </summary>
    public class DeviceSession
    {
        #region String Constants
        const byte DEVICE_TYPE_INDEX = 6;
        const byte DEVICE_ID_MSB_INDEX = 5;
        const byte DEVICE_ID_LSB_INDEX = 4;
        const byte POWER_METER_DEVICE_TYPE = 0x0B;
        const byte FEC_DEVICE_TYPE = 0x11;
        #endregion

        private List<Message> messages;

        public ushort EmotionId { get; private set; }
        public ushort PowerMeterId { get; private set; }
        public byte ChannelId { get; private set; }
        
        public IEnumerable<Message> Messages
        {
            get { return messages; }
        }

        public override string ToString()
        {
            const string TO_STRING_TEMPLATE = "DeviceId: {0}, ChannelId: {1}, Messages: {2}";
            return string.Format(TO_STRING_TEMPLATE, PowerMeterId, ChannelId, messages.Count);
        }

        public void AddMessage(Message message)
        {
            // Ignore the message if it's not a channelid message and we don't yet have a ChannelId yet
            messages.Add(message);
        }

        public Message GetLastMessage()
        {
            if (messages.Count > 0)
                return messages[messages.Count - 1];
            else
                return null;
        }

        public static DeviceSession GetDeviceSession(Message message)
        {
            DeviceSession session = null;

            // Start a session when we see a set channel id for a power meter type.
            if (message.ChannelId != null && message.EventId == (byte)
                    ANT_Managed_Library.ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51)
            {
                if (message.Bytes[DEVICE_TYPE_INDEX] == POWER_METER_DEVICE_TYPE)
                {
                    ushort deviceId = (ushort)(message.Bytes[DEVICE_ID_MSB_INDEX] << 8 |
                        message.Bytes[DEVICE_ID_LSB_INDEX]);

                    session = new DeviceSession((byte)message.ChannelId, deviceId);
                }
                //else if (message.Bytes[DEVICE_TYPE_INDEX] == FEC_DEVICE_TYPE)
                //{

                //}
            }

            return session;
        }

        private DeviceSession(byte channelId, ushort deviceId)
        {
            this.ChannelId = channelId;
            this.PowerMeterId = deviceId;
            messages = new List<Message>();
        }
    }
}
