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

        public System.Collections.IEnumerable FindAllFecEvents()
        {
            var messages = from message in session.Messages
                           where session.FecId > 0 && session.FecChannelId == message.GetChannelId() &&
                               message.GetMessageId() > 0
                           select FecMessage.GetFecData(message);

            return messages;
        }


        public IEnumerable<Message> FindAllFecResistanceCommands()
        {
            const byte TRAINER_MESSAGE_MASK = 0x30;
            const byte TRAINER_MESSAGE_TYPES = 4;

            var messages = from message in session.Messages
                           where session.FecId > 0 && session.FecChannelId == message.GetChannelId() &&
                               message.IsTransmit() && 
                               ((message.GetMessageId() ^ TRAINER_MESSAGE_MASK) < TRAINER_MESSAGE_TYPES)
                           select message;
            return messages;
        }

        public IEnumerable<Message> FindAllPowerMeterBroadcastEvents()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsDataMessage() &&
                    message.GetChannelId() == session.PowerMeterChannelId &&
                    message.GetMessageId() < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return messages;
        }
    }
}
