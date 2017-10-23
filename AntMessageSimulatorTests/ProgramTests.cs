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
    public class ProgramTests
    {
        [TestMethod()]
        public void ParseDeviceLogTest()
        {
            string path = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
            List<DeviceSession> sessions = PowerMeterSimulator.ParseDeviceLog(path);
            Assert.IsTrue(sessions.Count > 7);
        }
    }
}