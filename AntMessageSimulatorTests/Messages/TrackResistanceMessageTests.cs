using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator.Messages.Fec;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class TrackResistanceMessageTests
    {
        [TestMethod()]
        public void TrackResistanceMessageTest()
        {
            const string line = "    93.469 { 101940406} Tx - [A4][09][4F][03][33][FF][FF][FF][FF][08][4E][FF][6B][00][00]";
            TrackResistanceMessage message = (TrackResistanceMessage)MessageFactory.MessageFromLine(line);
            Assert.IsNotNull(message);
            Assert.IsTrue(message.CoEff == 255);
            Assert.IsTrue(message.Grade == -0.240004465F);
        }
    }
}