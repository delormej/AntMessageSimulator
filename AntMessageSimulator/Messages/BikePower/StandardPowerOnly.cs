using System;

namespace AntMessageSimulator.Messages.BikePower
{
    public class StandardPowerOnly : Message
    {
        public new const byte Page = 0x10;

        public StandardPowerOnly(Message message) : base(message)
        {
            
        }
    }
}
