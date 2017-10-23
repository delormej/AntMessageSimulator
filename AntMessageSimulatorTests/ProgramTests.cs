﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            List<DeviceSession> sessions = DeviceLogParser.Parse(path);
            Assert.IsTrue(sessions.Count > 7);
        }
    }
}