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
        [TestMethod()]
        public void GetGeneralFeDataTest()
        {
            string line = "  4069.735 { 187267906} Rx - [A4][14][4E][02][10][19][00][00][00][00][FF][24][E0][89][E0][11][05][10][00][69][00][C0][C0][CA]";
            Message message = MessageFactory.MessageFromLine(line);
            var data = FecMessage.GetGeneralFeData(message);
            float timestamp = GetDynamicObjectProperty<float>(data, "Timestamp");
            float speed = GetDynamicObjectProperty<float>(data, "Speed");
            Assert.IsTrue(timestamp == 4069.735F);
            Assert.IsTrue(speed == 0.0F);
        }

        public static T GetDynamicObjectProperty<T>(object o, string property)
        {
            object value = o?.GetType().GetProperty(property)?.GetValue(o, null);
            return (T)value;
        }

    }
}