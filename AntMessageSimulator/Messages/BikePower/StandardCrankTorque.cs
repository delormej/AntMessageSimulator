using System;
using System.Collections.Generic;

namespace AntMessageSimulator.Messages.BikePower
{
    public class StandardCrankTorque : Message
    {
        public new const byte Page = 0x12;

        public byte EventCount { get; private set; }
        public byte CrankTicks { get; private set; }
        public byte InstantCadence { get; private set; }
        public ushort Period { get; private set; } // 1/2048s
        public ushort AccumulatedTorque { get; private set; }
        public ushort CalculatedPower { get; private set; }

        public StandardCrankTorque(Message message) : base(message)
        {
            EventCount = message.Bytes[5];
            CrankTicks = message.Bytes[6];
            InstantCadence = message.Bytes[7];
            Period = (ushort)(message.Bytes[9] << 8 | message.Bytes[8]);
            AccumulatedTorque = (ushort)(message.Bytes[11] << 8 | message.Bytes[10]); 
        }

        public override void OnAddedToCollection(in List<Message> messages)
        {
            var lastMessage = messages.FindLast(FindLastStandardCrankTorque) as StandardCrankTorque;

            if (lastMessage != null)
                CalculatedPower = lastMessage.EventCount; // hack for now.
        }

        private bool FindLastStandardCrankTorque(Message message)
        {
            StandardCrankTorque torqueMessage = message as StandardCrankTorque;
            if (torqueMessage == null)
                return false;
            return (torqueMessage.EventCount< this.EventCount); // TODO: this is not accurate, because we need to deal with rollover.
        }
    }
}
