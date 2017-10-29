using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AntMessageSimulator
{
    // TODO: Should these all be static methods instead? 
    public class MessageQuery
    {
        DeviceSession session;

        public MessageQuery(DeviceSession session)
        {
            this.session = session;
        }

        public IEnumerable FindAllFecEvents()
        {
            foreach (var message in FindAllFecMessages())
                yield return FecMessage.GetFecData(message);
        }

        public IEnumerable<Message> FindAllFecMessages()
        {
            var messages = from message in session.Messages
                           where session.FecId > 0 && message.IsDataMessage() &&
                           session.FecChannelId == message.ChannelId
                           select message;

            return NotNullItems(messages);
        }

        public IEnumerable<Message> FindAllFecResistanceCommands()
        {
            const byte TRAINER_MESSAGE_MASK = 0x30;
            const byte TRAINER_MESSAGE_TYPES = 4;

            var messages = from message in session.Messages
                           where session.FecId > 0 && message.IsDataMessage() &&
                           session.FecChannelId == message.ChannelId && message.IsTransmit() && 
                               ((message.GetMessageId() ^ TRAINER_MESSAGE_MASK) < TRAINER_MESSAGE_TYPES)
                           select message;
            return NotNullItems(messages);
        }

        public IEnumerable<Message> FindAllPowerMeterBroadcastEvents()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsDataMessage() &&
                    message.ChannelId == session.PowerMeterChannelId &&
                    message.GetMessageId() < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return NotNullItems(messages);
        }

        public IEnumerable FindAllGeneralFeMessages()
        {
            return from message in FindAllFecMessages()
                         where message.IsGeneralFeData()
                         select FecMessage.GetGeneralFeData(message);
        }

        private IEnumerable<T> NotNullItems<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
                if (item != null)
                    yield return item;
        }
    }
}
