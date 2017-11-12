using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class ExecutionOptionsTests
    {
        static readonly string inputFile = "..\\..\\..\\AntMessageSimulatorTests\\Device0.txt";
        static readonly string outputFile = "..\\..\\..\\AntMessageSimulatorTests\\Device0.json";
        static readonly string[] args = {
                inputFile,
                "7",    // session #
                outputFile,
                "--fec"};

        [TestMethod()]
        public void ExecutionOptionsSummaryTest()
        {
            ExecutionOptions options = new ExecutionOptions(new string[] { inputFile });
            Assert.IsTrue(options.Operation == OperationType.SummaryOnly);
            Assert.IsFalse(options.WriteToFile);
            Assert.IsTrue(options.SessionNumber == 0);
        }
    }
}