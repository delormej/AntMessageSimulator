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
    public class MessageQueryTests
    {
        [TestMethod()]
        public void FindAllGeneralFeMessagesTest()
        {
            string path = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
            DeviceLogParser parser = new DeviceLogParser();
            List<DeviceSession> sessions = parser.Parse(path);

            MessageQuery query = new MessageQuery(sessions[7]);
            IEnumerable<SpeedEvent> events = 
                (IEnumerable<SpeedEvent>)query.FindAllGeneralFeMessages();
            float sum = events.Sum(s => s.Speed);
            Assert.IsTrue(sum > 1000F);
            Assert.IsTrue(events.Count() == 6463);
        }
    }
}