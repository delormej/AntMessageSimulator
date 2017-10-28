using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AntMessageSimulator.Tests
{
    public class TestSetup
    {
        public static List<DeviceSession> GetSessions()
        {
            string path = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
            DeviceLogParser parser = new DeviceLogParser();
            return parser.Parse(path);
        }
    }

    [TestClass()]
    public class DeviceLogParserTests
    {
        [TestMethod()]
        public void ParseDeviceLogTest()
        {
            List<DeviceSession> sessions = TestSetup.GetSessions();
            Assert.IsTrue(sessions.Count > 7);
            Assert.IsTrue(sessions[7].GetSessionDuration().Minutes == 12);
        }
    }
}