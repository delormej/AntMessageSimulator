using System;
using System.Collections.Generic;
using System.Linq;

namespace AntMessageSimulator
{
    /// <summary>
    /// Encapsulates logic to finds the relevant events to send over the wire.
    /// For instance, if the rollers are attached, only send events when there is valid
    /// speed.
    /// </summary>
    public class PowerMeterEventsQuery
    {
        DeviceSession session;

        public PowerMeterEventsQuery(DeviceSession session)
        {
            this.session = session;
        }

        //public IEnumerable<Message> FindEventsWithSpeed()
        //{
        //    return null;
        //}

        public IEnumerable<Message> FindAllFecResistanceCommands()
        {
            var messages = from message in session.Messages
                           where session.FecId > 0 && session.FecChannelId == message.GetChannelId() &&
                               message.IsTransmit() /*&& ((message.GetMessageId() ^ 0x48) < 4)*/
                           select message;
            return messages;
        }

        public IEnumerable<Message> FindAllPowerMeterBroadcastEvents()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsBroadcastEvent() &&
                    message.GetChannelId() == session.PowerMeterChannelId &&
                    message.GetMessageId() < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return messages;
        }
    }
}
