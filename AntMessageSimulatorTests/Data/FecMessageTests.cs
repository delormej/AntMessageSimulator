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
    public class FecMessageTests
    {
        DeviceSession session;

        [TestInitialize]
        public void Setup()
        {
            session = TestSetup.GetSessions()[6];
        }

        [TestMethod()]
        public void GetSpecificTrainerDataTest()
        {
            string line = "  2176.203 { 410420203} Rx - [A4][14][4E][01][19][7F][FF][B7][00][CB][00][31][E0][89][E0][11][05][10][00][68][00][6B][CD][68]";
            Message message = Message.MessageFromLine(line);
            var data = FecMessage.GetSpecificTrainerData(message);
            Assert.IsTrue(GetDynamicObjectProperty<int>(data, "InstantPower") == 203);
        }

        [TestMethod()]
        public void GetGeneralFeDataTest()
        {
            Assert.Fail();
        }

        public static T GetDynamicObjectProperty<T>(object o, string property)
        {
            return (T)o?.GetType().GetProperty(property)?.GetValue(o, null);
        }

    }
}