
namespace AntMessageSimulator.Messages.Fec
{
    public class TrackResistanceMessage : Message
    {
        public new const byte Page = 0x33;

        public float Grade { get; private set; }
        public byte CoEff { get; private set; }

        public TrackResistanceMessage(Message message) : base(message)
        {
            if (message.MessageId != Page)
                throw new ApplicationException("Not a valid Track Resistance data message.");

            Grade = DecodeGrade(message);
            CoEff = DecodeCoEff(message);
        }

        public static float DecodeGrade(Message message)
        {
            ushort value = (ushort)(message.Bytes[5 + 4] | (message.Bytes[6 + 4] << 8));
            //value ^= 1 << 15;
            //float grade = value / 32768.0f;
            float grade = (value * 0.01F) - 200.0F;

            return grade;
        }

        private byte DecodeCoEff(Message message)
        {
            return message.Bytes[7 + MESSAGE_HEADER_LENGTH];
        }

        public override string ToString()
        {
            const string format = "{{ Timestamp = {0:F3}, Grade = {1:F3}, CoEff = {2} }}";
            return string.Format(format, Timestamp, Grade, CoEff);
        }
    }
}
