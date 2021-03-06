﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class DeviceSessionTests
    {
        [TestMethod()]
        public void GetDeviceIdTest()
        {
            string line = "    31.859 {2319279593} Tx - [A4][05][51][01][E6][01][0B][00][1D][00][00]";
            var message = MessageFactory.MessageFromLine(line);

            Assert.IsTrue(message.MessageId == 0);
            Assert.IsTrue(message.DeviceId == 486);
            Assert.IsTrue(message.ChannelId == 1);
        }
    }
}