using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.Collections.Generic;
using AntMessageSimulator.Messages.Fec;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class IrtPowerAdjustMessageTest
    {
        [TestMethod()]
        public void ToStringTest()
        {
            const string line = "  3457.453 {1698286484} Rx - [A4][09][4E][01][F3][34][D4][05][03][02][1F][FF][15]";
            IrtPowerAdjustMessage message = MessageFactory.MessageFromLine(line) as IrtPowerAdjustMessage;
            Assert.IsNotNull(message);
            Assert.IsTrue(message.PowerMeterId == 54324);
        }
    }
}