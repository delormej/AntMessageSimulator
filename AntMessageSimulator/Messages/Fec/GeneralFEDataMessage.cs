
namespace AntMessageSimulator.Messages.Fec
{
    public class GeneralFEDataMessage : Message, SpeedEvent
    {
        public new const byte Page = 0x10;

        public float Speed { get; private set; }
        public float AverageSpeed { get; private set; }

        private static ChannelAverager speedAverager = new ChannelAverager();

        public GeneralFEDataMessage(Message message) : base(message)
        {
            if (message.MessageId != Page)
                throw new ApplicationException("Not a valid General FE data message.");
            Speed = GetSpeedMps(message);
            AverageSpeed = (float)speedAverager.Average(message.ChannelId, Speed);
        }

        public override string ToString()
        {
            return string.Format("{{ Timestamp = {0:F3}, Speed = {1:F2}, AverageSpeed = {2:F2} }}", 
                Timestamp, Speed, AverageSpeed);
        }

        private float GetSpeedMps(Message message)
        {
            float speedMps = (message.Bytes[4 + MESSAGE_HEADER_LENGTH] | 
                (message.Bytes[5 + MESSAGE_HEADER_LENGTH] << 8)) / 1000F;
            return speedMps;
        }
    }
}
