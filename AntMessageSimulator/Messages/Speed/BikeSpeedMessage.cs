
namespace AntMessageSimulator.Messages.Speed
{
    //
    //
    // NOTE: this is not currently used as GeneralFEData has speed.
    //
    //
    public class BikeSpeedMessage : Message
    {
        public new const byte Page = 0x03;

        public ushort EventTime { get; private set; }
        public ushort RevCount { get; private set; }

        public BikeSpeedMessage(Message message) : base(message)
        {
            EventTime = (ushort)(message.Bytes[9] << 8 | message.Bytes[8]);
            RevCount = (ushort)(message.Bytes[11] << 8 | message.Bytes[10]); 
        }

        public override Reading Accept(CreateReadingVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
