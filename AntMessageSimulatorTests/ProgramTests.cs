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
            List<PowerMeterSession> sessions = Program.ParseDeviceLog(path);
            Assert.IsTrue(sessions.Count > 7);
        }
    }
}