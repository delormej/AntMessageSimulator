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
    public class SpecificTrainerDataMessageTests
    {
        [TestMethod()]
        public void SpecificTrainerDataMessageTest()
        {
            const string line = "  2422.218 { 410666218} Rx - [A4][14][4E][01][19][69][FF][C5][EA][3C][00][31][E0][89][E0][11][05][10][00][69][00][D6][CE][AE]";
            SpecificTrainerDataMessage message = (SpecificTrainerDataMessage)MessageFactory.MessageFromLine(line);
            Assert.IsNotNull(message);
            Assert.IsFalse(message.IsTransmit);
            Assert.IsTrue(message.TargetPowerLimits == 1);
            Assert.IsTrue(message.TrainerFEState == 3);
            Assert.IsTrue(message.InstantPower == 60);
        }

        [TestMethod()]
        public void GetSpecificTrainerDataTest()
        {
            string line = "  2176.203 { 410420203} Rx - [A4][14][4E][01][19][7F][FF][B7][00][CB][00][31][E0][89][E0][11][05][10][00][68][00][6B][CD][68]";
            Message message = MessageFactory.MessageFromLine(line);
            SpecificTrainerDataMessage data = new SpecificTrainerDataMessage(message);
            Assert.IsTrue(data.InstantPower == 203);
        }
    }
}