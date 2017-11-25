using System.Linq;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class PowerAverager
    {
        const int QUEUE_LENGTH = 4;
        private Queue<short> values = new Queue<short>(QUEUE_LENGTH);

        public double Average(short instantPower)
        {
            values.Enqueue(instantPower);
            double average = values.Average(m => (int)m);
            if (values.Count > QUEUE_LENGTH)
                values.Dequeue(); // drop the oldest 
            return average;
        }
    }
}
