
namespace AntMessageSimulator.Messages.Common
{
    public class Util
    {
        /// <summary> This method accumulates a double byte into a 16 bit unsigned int. </summary>
        public static ushort AccumulateDoubleByte(ushort accumulator, ushort value)
        {
            // Did a rollover occur?
            if (value < (accumulator & 0xFFFF))
            {
                accumulator += 0xFFFF;
            }
            accumulator += value;

            return accumulator;
        }

        /// <summary> This method accumulates a single byte into an 8 bit unsigned int (byte). </summary>
        public static byte AccumulateByte(byte accumulator, byte value)
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