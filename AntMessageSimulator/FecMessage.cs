using System;

namespace AntMessageSimulator
{
    public class FecMessage
    {
        /// <summary>
        /// Parses the Irt Extra info message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static object GetIrtInfoFromMessage(Message message)
        {
            if (message.GetMessageId() != 0xF1)
                throw new ApplicationException("Not a valid Irt Extra Info message.");

            var data = new
            {
                Timestamp = message.Timestamp,
                ServoPosition = message.Bytes[1 + 4] | (message.Bytes[2 + 4] << 8),
                Target = message.Bytes[3 + 4] | (message.Bytes[4 + 4] << 8),
                FlyWheel = message.Bytes[5 + 4] | (message.Bytes[6 + 4] << 8),
                PowerMeterPaired = (message.Bytes[7 + 4] & 0x80) >> 7
            };

            return data;
        }

        public static object GetSpecificTrainerData(Message message)
        {
            if (message.GetMessageId() != 0x19)
                throw new ApplicationException("Not a valid Specific Trainer data message.");

            var data = new
            {
                Timestamp = message.Timestamp,
                // Uses 1.5 bytes
                InstantPower = (message.Bytes[6 + 4] & 0x0F << 8) | message.Bytes[5 + 4]
            };

            return data;
        }

        public static object GetGeneralFeData(Message message)
        {
            if (message.GetMessageId() != 0x10)
                throw new ApplicationException("Not a valid General FE data message.");

            var data = new
            {
                Timestamp = message.Timestamp,
                Speed = (message.Bytes[4 + 4] | (message.Bytes[5 + 4] << 8)) / 1000F
            };

            return data;
        }

        public static object GetFecData(Message message)
        {
            object data = null;

            // Determine the message type
            switch (message.GetMessageId())
            {
                case 0xF1: // IrtExtraInfoPage
                    data = GetIrtInfoFromMessage(message);
                    break;
                case 0x19: // SpecificTrainerDataPage
                    data = GetSpecificTrainerData(message);
                    break;
                case 0x10:
                    data = GetGeneralFeData(message);
                    break;
                default:
                    data = null;
                    break;
            }

            return data;
        }
    }
}
