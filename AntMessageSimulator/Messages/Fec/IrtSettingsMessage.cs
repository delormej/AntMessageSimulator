
namespace AntMessageSimulator.Messages.Fec
{
    public class IrtSettingsMessage : Message
    {
        public new const byte Page = 0xF2;

        public IrtSettingsMessage(Message message) : base(message)
        {
            if (message.MessageId != Page)
                throw new ApplicationException("Not a valid Irt Settings message.");

            Drag = Decode(message, 1);
            RollingResistance = Decode(message, 3);
            ServoOffset = Decode(message, 5);
            Settings = message.Bytes[7];
        }

        public ushort Drag { get; private set; }
        public ushort RollingResistance{ get; private set; }
        public ushort ServoOffset { get; private set; }
        public byte Settings{ get; private set; }

        private static ushort Decode(Message message, int offset)
        {
            return (ushort)(message.Bytes[offset + MESSAGE_HEADER_LENGTH] |
                (message.Bytes[offset + 1 + MESSAGE_HEADER_LENGTH] << 8));
        }


        public override string ToString()
        {
            const string format = "{{ Timestamp = {0:F3}, Drag = {1:G}, RollingResistance = {2:G}, ServoOffset = {3:G}, Settings = {4} }}";
            return string.Format(format,
                Timestamp,
                Drag,
                RollingResistance,
                ServoOffset,
                Settings);
        }
    }
}
