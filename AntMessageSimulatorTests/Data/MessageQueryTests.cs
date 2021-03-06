﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class MessageQueryTests
    {
        List<DeviceSession> sessions;

        [TestInitialize]
        public void Setup()
        {
            sessions = TestSetup.GetSessions();
        }

        [TestMethod()]
        public void FindAllGeneralFeMessagesTest()
        {
            MessageQuery query = new MessageQuery(sessions[7]);
            IEnumerable<SpeedEvent> events =
                (IEnumerable<SpeedEvent>)query.FindAllGeneralFeMessages();
            float sum = events.Sum(s => s.Speed);
            Assert.IsTrue(sum == 59067.4961F);
            Assert.IsTrue(events.Count() == 6463);
        }

        [TestMethod()]
        public void FindAllFecMessagesTest()
        {
            var messages = sessions[7].Messages.AsQueryable().Where("Timestamp > 300 and Timestamp < 310", null);
            Assert.IsTrue(messages.Count() > 190);
        }

        [TestMethod()]
        public void FindAllFecTransmitMessagesTest()
        {
            MessageQuery query = new MessageQuery(sessions[7]);
            IEnumerable<Message> events = query.FindAllFecMessages();
            int count = events.Count();
            Assert.IsTrue(events.Count() == 17406);
        }
    }
}