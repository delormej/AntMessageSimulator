using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AntMessageSimulator
{
    /// <summary>
    /// Encapsulates logic to finds the relevant events to send over the wire.
    /// For instance, if the rollers are attached, only send events when there is valid
    /// speed.
    /// </summary>
    public class MessageQuery
    {
        DeviceSession session;

        public MessageQuery(DeviceSession session)
        {
            this.session = session;
        }
        // TODO: implement method to get speed events.
        //public IEnumerable<Message> FindEventsWithSpeed()

        public IEnumerable FindAllFecEvents()
        {
            foreach (var message in FindAllFecMessages())
                yield return FecMessage.GetFecData(message);
        }

        public IEnumerable<Message> FindAllFecMessages()
        {
            var messages = from message in session.Messages
                           where session.FecId > 0 && session.FecChannelId == message.GetChannelId() &&
                               message.GetMessageId() > 0
                           select message;

            return NotNullItems(messages);
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
            return NotNullItems(messages);
        }

        public IEnumerable<Message> FindAllPowerMeterBroadcastEvents()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsDataMessage() &&
                    message.GetChannelId() == session.PowerMeterChannelId &&
                    message.GetMessageId() < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return NotNullItems(messages);
        }

        private IEnumerable<T> NotNullItems<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
                if (item != null)
                    yield return item;
        }
    }
}
