using System;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    /// <summary>
    /// Represents a ride session as built from a device log.
    /// </summary>
    public class DeviceSession
    {
        private List<Message> messages;

        public ushort FecId { get; private set; }
        public byte FecChannelId { get; private set; }
        public ushort PowerMeterId { get; private set; }
        public byte PowerMeterChannelId { get; private set; }

        public IEnumerable<Message> Messages
        {
            // Do not allow changes to the underlying collection (List<T>), so 
            // just expose IEnumerable<T> to allow interation.
            get { return messages; }
        }

        public bool IsValid()
        {
            // Valid sessions must have a power meter OR FE-C associated.
            return (PowerMeterId > 0 || FecId > 0);
        }

        public override string ToString()
        {
            const string TO_STRING_TEMPLATE = "Duration: {5:h\\:mm\\:ss}, DeviceId: {0}, " +
                "ChannelId: {1}, FecId: {2}, FecChannelId: {3}, Messages: {4}";
            return string.Format(TO_STRING_TEMPLATE, 
                PowerMeterId, PowerMeterChannelId, FecId, FecChannelId, messages.Count, 
                GetSessionDuration());
        }

        public TimeSpan GetSessionDuration()
        {
            float seconds = 0f;
            if (messages.Count > 0)
                seconds = messages[messages.Count - 1].Timestamp - messages[0].Timestamp;

            return TimeSpan.FromSeconds(seconds);
        }

        public void AddMessage(Message message)
        {
            /*
             * It seems like this should be more extensible, without needing to change this
             * method.  Should we be raising an event here? Clearly other events will be interesting.
             */

            if (PowerMeterId == 0 && message.IsPowerMeterIdEvent())
            {
                PowerMeterChannelId = message.ChannelId;
                PowerMeterId = message.GetDeviceId();
            }
            else if (FecId == 0 && message.IsFecIdEvent())
            {
                FecChannelId = message.ChannelId;
                FecId = message.GetDeviceId();
            }

            messages.Add(message);
        }

        public float GetLastTimestamp()
        {
            float timestamp = 0;
            if (messages.Count > 0)
            {
                Message message = messages[messages.Count - 1];
                timestamp = message.Timestamp;
            }

            return timestamp;
        }
        
        public DeviceSession()
        {
            messages = new List<Message>();
        }            
    }
}
