using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod()]
        public void ToAutoAntScriptLineTest()
        {
            string line = "   6964.313 {  88559625} Rx - [A4][14][4E][01][20][C4][00][D5][75][B3][FC][67][E0][E6][01][0B][01][10][00][69][00][3B][44][98]";
            Message message = Message.MessageFromLine(line);
            string script = message.ToAutoAntScriptLine();
            const string expected = "w [4E][01][20][C4][00][D5][75][B3][FC][67]\r\nr [40][01][20][03]\r\n";

            Assert.AreEqual<string>(expected, script);
        }
    }
}