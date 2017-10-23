using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void ParseDeviceLogTest()
        {
            string path = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
            DeviceLogParser parser = new DeviceLogParser();
            List<DeviceSession> sessions = parser.Parse(path);
            Assert.IsTrue(sessions.Count > 7);
        }
    }
}