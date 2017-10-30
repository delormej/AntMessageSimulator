using System;

namespace AntMessageSimulator
{
    public class SpeedEvent
    {
        public float Timestamp { get; set; }
        public float Speed { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Timestamp = {0:F3}, Speed = {1:F2} }}", Timestamp, Speed);
        }
    }
}
