using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class CommandStatusMessageTests
    {
        [TestMethod()]
        public void CommandStatusMessageTest()
        {
            const string line = "    42.312 {2319290046} Rx - [A4][14][4E][00][47][33][FF][00][FF][EB][4D][FF]";
            CommandStatusMessage message = (CommandStatusMessage)MessageFactory.MessageFromLine(line);
            if (message.LastCommand == 0x33)
                Assert.IsTrue(-0.530004442F == TrackResistanceMessage.DecodeGrade(message));
            else
                Assert.Fail();
        }
    }
}