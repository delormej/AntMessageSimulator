using System;

namespace AntMessageSimulator
{
    public class FecMessage
    {
        public static SpeedEvent GetGeneralFeData(Message message)
        {
            if (message.MessageId != 0x10)
                throw new ApplicationException("Not a valid General FE data message.");

            var data = new SpeedEvent()
            {
                Timestamp = message.Timestamp,
                Speed = (message.Bytes[4 + 4] | (message.Bytes[5 + 4] << 8)) / 1000F
            };

            return data;
        }

        private static float DecodeGrade(Message message)
        {
            ushort value = (ushort)(message.Bytes[5 + 4] | (message.Bytes[6 + 4] << 8));
            //value ^= 1 << 15;
            //float grade = value / 32768.0f;
            float grade = (value * 0.01F) - 200.0F;

            return grade;
        }

        private static object GetPrintableMessage(Message message)
        {
            return new { Timestamp = message.Timestamp, Payload = message.GetPayloadAsString() };
        }

        public static object GetTrackResistanceData(Message message)
        {
            if (message.MessageId != 0x33)
                throw new ApplicationException("Not a valid Track Resistance data message.");

            var data = new
            {
                Timestamp = message.Timestamp,
                Grade = DecodeGrade(message),
                CoEff = message.Bytes[7 + 4]
            };

            return data;
        }

        public static object GetCommandStatusData(Message message)
        {
            if (message.MessageId != 0x47)
                throw new ApplicationException("Not a valid Command Status data message.");

            var data = new
            {
                Timestamp = message.Timestamp,
                LastCommand = string.Format("{0:X2}", message.Bytes[1 + 4]),
                Status = message.Bytes[3 + 4],
                Grade = DecodeGrade(message),    // hack: this is only good if lastcommand = 0x33.
                CoEff = message.Bytes[7 + 4]
            };

            return data;
        }

        public static object GetFecData(Message message)
        {
            object data = null;

            // Determine the message type
            switch (message.MessageId)
            {
                case 0xF1: // IrtExtraInfoPage
                    //data = GetIrtInfoFromMessage(message);
                    throw new NotImplementedException();
                    break;
                case 0x19: // SpecificTrainerDataPage
                    throw new NotImplementedException();
                    break;
                case 0x10:
                    data = GetGeneralFeData(message);
                    break;
                case 0x33:
                    data = GetTrackResistanceData(message);
                    break;
                case 0x47:
                    data = GetCommandStatusData(message);
                    break;
                default:
                    // just default to a normal message if we can't identify it.
                    data = GetPrintableMessage(message);
                    break;
            }

            return data;
        }
    }
}
