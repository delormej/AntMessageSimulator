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
    public class MessageTests
    {
        [TestMethod()]
        public void MessageFromLineTest()
        {
            string line = "    32.859 {2319280593} Rx - [A4][14][4E][01][01][10][01][00][00][00][02][4B][E0][E6][01][0B][01][10][00][6D][00][5A][42][CE]";
            Message message = Message.MessageFromLine(line);
            Assert.IsTrue(message.ChannelId == 1);
            Assert.IsTrue(message.Timestamp == 32.859F);
        }
    }
}