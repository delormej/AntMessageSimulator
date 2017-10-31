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
    public class WaveGeneratorTests
    {
        [TestMethod()]
        public void CreateScriptTest()
        {
            List<DeviceSession> sessions = TestSetup.GetSessions();
            WaveGenerator generator = new WaveGenerator(sessions[8]);
            string output = generator.Generate();

            Assert.IsNotNull(output);
        }
    }
}