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

        public override string ToString()
        {
            const string TO_STRING_TEMPLATE = "DeviceId: {0}, ChannelId: {1}, FecId: {2}, " + 
                "FecChannelId: {3}, Messages: {4}";
            return string.Format(TO_STRING_TEMPLATE, 
                PowerMeterId, PowerMeterChannelId, FecId, FecChannelId, messages.Count);
        }

        public void AddMessage(Message message)
        {
            /*
             * It seems like this should be more extensible, without needing to change this
             * method.  Should we be raising an event here? Clearly other events will be interesting.
             */

            if (PowerMeterId == 0 && message.IsPowerMeterIdEvent())
            {
                PowerMeterChannelId = (byte)message.GetChannelId();
                PowerMeterId = message.GetDeviceId();
            }
            else if (FecId == 0 && message.IsFecIdEvent())
            {
                FecChannelId = (byte)message.GetChannelId();
                FecId = message.GetDeviceId();
            }

            messages.Add(message);
        }

        public Message GetLastMessage()
        {
            if (messages.Count > 0)
                return messages[messages.Count - 1];
            else
                return null;
        }
        
        public DeviceSession()
        {
            messages = new List<Message>();
        }            
    }
}
