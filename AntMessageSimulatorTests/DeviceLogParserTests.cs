using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AntMessageSimulator.Tests
{
    public class TestSetup
    {
        private static List<DeviceSession> sessions;
        private static object lockObject = new object();

        public static List<DeviceSession> GetSessions()
        {
            lock (lockObject)
            {
                if (sessions == null)
                {
                    string path = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
                    DeviceLogParser parser = new DeviceLogParser();
                    sessions = parser.Parse(path);
                }
                return sessions;
            }
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