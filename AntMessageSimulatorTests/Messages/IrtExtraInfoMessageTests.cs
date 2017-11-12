using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class IrtExtraInfoMessageTests
    {
        [TestMethod()]
        public void IrtExtraInfoMessageTest()
        {
            const string line = "  2421.468 { 410665468} Rx - [A4][14][4E][01][F1][D0][07][00][C0][46][B7][12][E0][89][E0][11][05][10][00][68][00][D5][6E][A4]";
            IrtExtraInfoMessage message = (IrtExtraInfoMessage)MessageFactory.MessageFromLine(line);
            Assert.IsNotNull(message);
            Assert.IsTrue(message.ServoPosition == 2000);
            Assert.IsTrue(message.Target == 0);
            Assert.IsTrue(message.TransmitType == TransmitType.Rx);
        }
    }
}