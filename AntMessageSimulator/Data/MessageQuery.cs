﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntMessageSimulator.Messages.Common;
using AntMessageSimulator.Messages.Fec;

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

        public IQueryable<Message> FindAllFecMessages()
        {
            var messages = from message in session.Messages
                           where session.FecId > 0 && message.IsDataMessage &&
                           session.FecChannelId == message.ChannelId
                           select message;

            return NotNullItems(messages);
        }

        public IQueryable<Message> FindAllFecTransmitMessages()
        {
            var messages = from message in session.Messages
                           where session.FecChannelId == message.ChannelId &&
                           message.IsDataMessage &&
                           message.IsTransmit
                           select message;
            return NotNullItems(messages);
        }

        public IQueryable<Message> FindAllPowerMeterBroadcastEvents()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsDataMessage &&
                    message.ChannelId == session.PowerMeterChannelId &&
                    message.MessageId < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return NotNullItems(messages);
        }

        public IEnumerable FindAllGeneralFeMessages()
        {
            return from message in FindAllFecMessages()
                         where message is GeneralFEDataMessage
                         select new GeneralFEDataMessage(message);
        }

        public string FindProductVersion(byte channelId)
        {
            List<Message> msgs = (from message in session.Messages
                                 where message.ChannelId == channelId
                                 select message).ToList();

            foreach (var msg in msgs)
            {
                ProductMessage message = msg as ProductMessage;
                if (message != null)
                    return message.Version;
            }

            return "";
                    //return (from message in session.Messages
                    //       where message.ChannelId == channelId &&
                    //       message is ProductMessage
                    //       select ((ProductMessage)message).Version).First();
        }

        public IQueryable<Message> FindAllPowerMeterAndFecMessages()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where IsMessageForDevice(message)
                select message;

            return NotNullItems(messages);
        }

        private bool IsMessageForDevice(Message message)
        {
            return (message.IsDataMessage &&
                (session.FecId > 0 && session.FecChannelId == message.ChannelId) ||
                (session.PowerMeterId > 0 && session.PowerMeterChannelId == message.ChannelId));
        }

        private IQueryable<T> NotNullItems<T>(IEnumerable<T> list)
        {
            return list.Where(l => l != null).AsQueryable();
        }
    }
}
