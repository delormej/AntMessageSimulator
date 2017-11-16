
namespace AntMessageSimulator
{
    public class IrtExtraInfoMessage : Message
    {
        public IrtExtraInfoMessage(Message message) : base(message)
        {
            if (message.MessageId != IRT_EXTRAINFO_PAGE)
                throw new ApplicationException("Not a valid Irt Extra Info message.");

            ServoPosition = DecodeServoPostion(message);
            Target = DecodeTarget(message);
            FlyWheel = DecodeFlyWheel(message);
            PowerMeterPaired = DecodePowerMeterPaired(message);
        }

        public ushort ServoPosition { get; private set; }
        public ushort Target { get; private set; }
        public ushort FlyWheel { get; private set; }
        public bool PowerMeterPaired { get; private set; }

        private static ushort DecodeServoPostion(Message message)
        {
            return (ushort)(message.Bytes[1 + MESSAGE_HEADER_LENGTH] |
                (message.Bytes[2 + MESSAGE_HEADER_LENGTH] << 8));
        }

        private static ushort DecodeTarget(Message message)
        {
            byte lsb = message.Bytes[3 + MESSAGE_HEADER_LENGTH];
            byte msb = message.Bytes[4 + MESSAGE_HEADER_LENGTH];
            // Encodes the resistance mode into the 2 most significant bits.
            ushort target = (ushort)(lsb | ((msb & 0x3F) << 8));
            return target;
        }

        private static ushort DecodeFlyWheel(Message message)
        {
            return (ushort)(message.Bytes[5 + MESSAGE_HEADER_LENGTH] | 
                (message.Bytes[6 + MESSAGE_HEADER_LENGTH] << 8));
        }

        private static bool DecodePowerMeterPaired(Message message)
        {
            return 1 == ((message.Bytes[7 + MESSAGE_HEADER_LENGTH] & 0x80) >> 7);
        }
        
        public override string ToString()
        {
            const string format = "{{ Timestamp = {0:F3}, ServoPosition = {1:G}, Target = {2:G}, Flywheel = {3:G}, PowerMeterPaired  = {4} }}";
            return string.Format(format,
                Timestamp,
                ServoPosition,
                Target,
                FlyWheel,
                PowerMeterPaired);
        }
    }
}
