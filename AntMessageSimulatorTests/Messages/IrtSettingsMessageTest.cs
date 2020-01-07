using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.Collections.Generic;
using AntMessageSimulator.Messages.Fec;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class IrtSettingsMessageTest
    {
        [TestMethod()]
        public void ToStringTest()
        {
            const string line = "    34.562 {   1133515} Rx - [A4][0E][4E][01][F2][56][5F][CF][46][B4][00][FF][80][8E][F3][11][05][35]";
            IrtSettingsMessage message = MessageFactory.MessageFromLine(line) as IrtSettingsMessage;
            Assert.IsNotNull(message);
            Assert.IsTrue(message.ServoOffset == 180);
        }
    }
}