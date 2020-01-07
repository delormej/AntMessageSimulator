
namespace AntMessageSimulator.Messages.Fec
{
    public class IrtPowerAdjustMessage : Message
    {
        public new const byte Page = 0xF3;

        public IrtPowerAdjustMessage(Message message) : base(message)
        {
            if (message.MessageId != Page)
                throw new ApplicationException("Not a valid Irt Power Adjust message.");

            PowerMeterId = Decode(message, 1);
            PowerAdjustSeconds = message.Bytes[3 + MESSAGE_HEADER_LENGTH];
            PowerAverageSeconds = message.Bytes[4 + MESSAGE_HEADER_LENGTH];
            ServoSmoothingSteps = message.Bytes[5 + MESSAGE_HEADER_LENGTH];
            MinAdjustSpeedMps = message.Bytes[6 + MESSAGE_HEADER_LENGTH];
        }

        public ushort PowerMeterId { get; private set; }
        public byte PowerAdjustSeconds { get; private set; }
        public byte PowerAverageSeconds { get; private set; }
        public byte ServoSmoothingSteps { get; private set; }
        public byte MinAdjustSpeedMps { get; private set; }

        private static ushort Decode(Message message, int offset)
        {
            return (ushort)(message.Bytes[offset + MESSAGE_HEADER_LENGTH] |
                (message.Bytes[offset + 1 + MESSAGE_HEADER_LENGTH] << 8));
        }


        public override string ToString()
        {
            const string format = "{{ Timestamp = {0:F3}, PowerMeterId = {1:G}, PowerAdjustSeconds = {2:G}, PowerAverageSeconds = {3:G}, ServoSmoothingSteps = {4:G}, MinAdjustSpeedMps = {5:G} }}";
            return string.Format(format,
                Timestamp,
                PowerMeterId,
                PowerAdjustSeconds,
                PowerAverageSeconds,
                ServoSmoothingSteps,
                MinAdjustSpeedMps);
        }
    }
}
