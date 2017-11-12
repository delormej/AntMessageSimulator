using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class GeneralFEDataMessageTests
    {
        [TestMethod]
        public void GeneralFEDataMessageTest()
        {
            //const string line = "  4069.735 { 187267906} Rx - [A4][14][4E][02][10][19][00][00][00][00][FF][24][E0][89][E0][11][05][10][00][69][00][C0][C0][CA]";
            const string line2 = "  2245.703 { 410489703} Rx - [A4][14][4E][01][10][19][D4][D4][A5][3C][FF][34][E0][89][E0][11][05][10][00][68][00][DB][8D][17]";
            GeneralFEDataMessage message = (GeneralFEDataMessage)MessageFactory.MessageFromLine(line2);
            Assert.IsTrue(message.Timestamp == 2245.703F);
            Assert.IsTrue(message.Speed == 15.525F);
        }
    }
}