using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class ProductMessageTests
    {
        [TestMethod()]
        public void ProductMessageTest()
        {
            const string line = "  4083.735 { 187281906} Rx - [A4][14][4E][02][51][FF][27][10][89][E0][41][D3][E0][89][E0][11][05][10][00][68][00][CD][C0][76]";
            ProductMessage message = (ProductMessage)MessageFactory.MessageFromLine(line);
            Assert.IsTrue(message.Version == "1.0.39");
        }
    }
}