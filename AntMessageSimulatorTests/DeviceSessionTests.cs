﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class DeviceSessionTests
    {
        [TestMethod()]
        public void GetDeviceSessionTest()
        {
            string line = "    31.859 {2319279593} Tx - [A4][05][51][01][E6][01][0B][00][1D][00][00]";
            var message = Message.MessageFromLine(line);
            var session = DeviceSession.GetDeviceSession(message);

            Assert.IsNull(message.MessageId);
            Assert.IsTrue(session.PowerMeterId == 486);
        }
    }
}