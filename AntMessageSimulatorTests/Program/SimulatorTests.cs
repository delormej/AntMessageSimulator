using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.IO;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class SimulatorTests
    {
        static readonly string inputFile = "..\\..\\..\\AntMessageSimulatorTests\\Device0.txt";
        static readonly string outputFile = "..\\..\\..\\AntMessageSimulatorTests\\Device0.json";
        static readonly string[] args = {
                inputFile,
                "7",    // session #
                outputFile,
                "--fec"};

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            ExecutionOptions options = new ExecutionOptions(args);
            Simulator simulator = new Simulator(options);
            simulator.Execute();
            Assert.IsTrue(File.Exists(outputFile));
        }
    }
}