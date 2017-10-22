using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class PowerMeterSessionTests
    {
        [TestMethod()]
        public void GetPowerMeterSessionTest()
        {
            string line = "    28.406 { 288576140} Tx - [A4][05][51][01][89][E0][11][00][89][00][00]";
            var message = Message.MessageFromLine(line);
            var session = PowerMeterSession.GetPowerMeterSession(message);

            Assert.IsTrue(session.DeviceId == 57481);
        }
    }
}