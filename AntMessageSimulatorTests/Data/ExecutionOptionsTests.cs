using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System.Collections;
using System.Linq;

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

        [TestMethod()]
        public void TestQuery()
        {
            string[] query = { "--q", "\"Timestamp > 300 and TimeStamp < 500.10\"" };
            string[] newArgs = args.Union(query).ToArray<string>();
            ExecutionOptions options = new ExecutionOptions(newArgs);
            Assert.IsTrue(options.Query == "Timestamp > 300 and TimeStamp < 500.10");
        }
    }
}