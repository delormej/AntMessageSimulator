using System;

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
            EventCount = GetEventCount(message.Bytes[2]);
            // pedalPower : future implement
            InstantCadence = message.Bytes[4];
            AccumulatedPower = GetAccumulatedPower((ushort)(message.Bytes[6] << 8 | message.Bytes[5]));
            InstantBikePower = (ushort)(message.Bytes[8] << 8 | message.Bytes[7]);
        }

        private byte GetEventCount(byte events)
        {
            eventCount = AccumulateByte(EventCount, events);
            return eventCount;
        }

        private ushort GetAccumulatedPower(ushort power)
        {
            accumulatedPower = AccumulateDoubleByte(accumulatedPower, power);
            return accumulatedPower;
        }

        private static ushort AccumulateDoubleByte(ushort accumulator, ushort value)
        {
            // Did a rollover occur?
            if (value < (accumulator & 0xFFFF))
            {
                accumulator += 0xFFFF;
            }
            accumulator += value;

            return accumulator;
        }

        // This method accumulates a single byte into a 32 bit unsigned int.
        private static byte AccumulateByte(byte accumulator, byte value)
        {
            // Did a rollover occur?
            if (value < (accumulator & 0xFF))
            {
                accumulator += 0xFF;
            }
            accumulator += value;
        
            return accumulator;
        }
    }
}
