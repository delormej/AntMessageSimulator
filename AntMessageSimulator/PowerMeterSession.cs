using System;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    /// <summary>
    /// Represents a ride session as built from a device log.
    /// </summary>
    public class PowerMeterSession
    {
        const byte DEVICE_TYPE_INDEX = 6;
        const byte DEVICE_ID_MSB_INDEX = 5;
        const byte DEVICE_ID_LSB_INDEX = 4;
        const byte POWER_METER_DEVICE_TYPE = 0x0B;

        public ushort DeviceId { get; private set; }
        public byte ChannelId { get; private set; }
        public List<Message> Messages { get; private set; }

        public static PowerMeterSession GetPowerMeterSession(Message message)
        {
            PowerMeterSession session = null;

            // Start a session when we see a set channel id for a power meter type.
            if (message.ChannelId != null && message.EventId == (byte)
                    ANT_Managed_Library.ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51)
            {
                if (message.Bytes[DEVICE_TYPE_INDEX] == POWER_METER_DEVICE_TYPE)
                {
                    ushort deviceId = (ushort)(message.Bytes[DEVICE_ID_MSB_INDEX] << 8 |
                        message.Bytes[DEVICE_ID_LSB_INDEX]);

                    session = new PowerMeterSession((byte)message.ChannelId, deviceId);
                }
            }

            return session;
        }

        private PowerMeterSession(byte channelId, ushort deviceId)
        {
            this.ChannelId = channelId;
            this.DeviceId = deviceId;
            this.Messages = new List<Message>();
        }
    }
}
