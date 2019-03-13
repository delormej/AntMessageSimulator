using System;
using AntMessageSimulator.Messages.Common;

namespace AntMessageSimulator.Messages.BikePower
{
    public class StandardPowerOnly : Message
    {
        public new const byte Page = 0x10;

        public byte EventCount { get; private set; }
        public byte InstantCadence { get; private set; }
        public ushort AccumulatedPower { get; private set; }
        public ushort InstantBikePower { get; private set; }

        static byte eventCount = 0;
        static ushort accumulatedPower = 0;

        public StandardPowerOnly(Message message) : base(message)
        {
            EventCount = GetEventCount(message.Bytes[5]);
            // pedalPower : future implement
            InstantCadence = message.Bytes[7];
            AccumulatedPower = GetAccumulatedPower((ushort)(message.Bytes[9] << 8 | message.Bytes[8]));
            InstantBikePower = (ushort)(message.Bytes[11] << 8 | message.Bytes[10]);
        }

        private byte GetEventCount(byte events)
        {
            eventCount = Util.AccumulateByte(EventCount, events);
            return eventCount;
        }

        private ushort GetAccumulatedPower(ushort power)
        {
            accumulatedPower = Util.AccumulateDoubleByte(accumulatedPower, power);
            return accumulatedPower;
        }
    }
}
