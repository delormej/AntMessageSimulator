using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class DeviceSessionTests
    {
        [TestMethod()]
        public void GetDeviceIdTest()
        {
            string line = "    31.859 {2319279593} Tx - [A4][05][51][01][E6][01][0B][00][1D][00][00]";
            var message = Message.MessageFromLine(line);

            Assert.IsTrue(message.GetMessageId() == 0);
            Assert.IsTrue(message.GetDeviceId() == 486);
            Assert.IsTrue(message.ChannelId == 1);
        }
    }
}