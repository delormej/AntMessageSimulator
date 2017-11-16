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
    public class GeneralSettingsMessageTests
    {
        [TestMethod()]
        public void ToStringTest()
        {
            const string line = "   300.500 {  80106421} Rx - [A4][14][4E][02][11][FF][FF][D1][FF][7F][55][34][E0][89][E0][11][05][10][01][6B][00][CD][74][29]";
            GeneralSettingsMessage message = MessageFactory.MessageFromLine(line) as GeneralSettingsMessage;
            Assert.IsNotNull(message);
            Assert.IsTrue(message.ResistanceLevel == 42.5F);
            
        }
    }
}