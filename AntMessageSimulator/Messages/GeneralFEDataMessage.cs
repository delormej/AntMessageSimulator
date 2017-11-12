
namespace AntMessageSimulator
{
    public class GeneralFEDataMessage : Message, SpeedEvent
    {
        public float Speed { get; private set; }

        public GeneralFEDataMessage(Message message) : base(message)
        {
            if (message.MessageId != 0x10)
                throw new ApplicationException("Not a valid General FE data message.");
            Speed = GetSpeedMps(message);
        }

        public override string ToString()
        {
            return string.Format("{{ Timestamp = {0:F3}, Speed = {1:F2} }}", Timestamp, Speed);
        }

        private float GetSpeedMps(Message message)
        {
            return (message.Bytes[4 + MESSAGE_HEADER_LENGTH] | 
                (message.Bytes[5 + MESSAGE_HEADER_LENGTH] << 8)) / 1000F;
        }
    }
}
