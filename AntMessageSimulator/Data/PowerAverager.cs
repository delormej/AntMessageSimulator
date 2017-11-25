using System.Linq;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public static class PowerAverager
    {
        const int QUEUE_LENGTH = 4;
        private static Queue<short> values = new Queue<short>(QUEUE_LENGTH);

        public static double Average(short instantPower)
        {
            values.Enqueue(instantPower);
            double average = values.Average(m => (int)m);
            if (values.Count > 4)
                values.Dequeue(); // drop the oldest 
            return average;
        }
    }
}
