using System.Linq;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class ChannelAverager
    {
        const int QUEUE_LENGTH = 4;
        private Dictionary<int, Queue<double>> queues;

        public ChannelAverager()
        {
            queues = new Dictionary<int, Queue<double>>();
        }

        public double Average(int channelId, double value)
        {
            Queue<double> values = GetQueueForChannel(channelId);
            values.Enqueue(value);
            double average = values.Average();
            if (values.Count > QUEUE_LENGTH)
                values.Dequeue(); // drop the oldest 
            return average;
        }

        private Queue<double> GetQueueForChannel(int channelId)
        {
            if (!queues.ContainsKey(channelId))
                queues.Add(channelId, new Queue<double>(QUEUE_LENGTH));
            return queues[channelId];
        }
    }
}
