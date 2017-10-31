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
    public class ChannelConfigurationTests
    {
        DeviceSession session;

        [TestInitialize]
        public void Setup()
        {
            session = TestSetup.GetSessions()[3];
        }

        [TestMethod()]
        public void ChannelConfigurationTest()
        {
            ChannelConfiguration config = new ChannelConfiguration(DeviceType.PowerMeter, session);
            Assert.IsTrue(config.ChannelId == 2);
            Assert.IsTrue(config.DeviceId == 486);
            Assert.IsTrue(config.DeviceIdLsb == 0xE6);
        }
    }
}