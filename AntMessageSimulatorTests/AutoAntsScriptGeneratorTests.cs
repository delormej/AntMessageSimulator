using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class AutoAntsScriptGeneratorTests
    {
        [TestMethod()]
        public void ToAutoAntScriptLineTest()
        {
            string line = "   6964.313 {  88559625} Rx - [A4][14][4E][01][20][C4][00][D5][75][B3][FC][67][E0][E6][01][0B][01][10][00][69][00][3B][44][98]";
            Message message = Message.MessageFromLine(line);
            string script = AutoAntsScriptGenerator.ToAutoAntScriptLine(message);
            const string expected = "w [4E][01][20][C4][00][D5][75][B3][FC][67]\r\nr [40][01][20][03]\r\n";

            // Just compare the first 10 chars.
            Assert.AreEqual<string>(expected.Substring(0, 10), script.Substring(0, 10));
        }

        [TestMethod()]
        public void CreateScriptStreamTest()
        {
            string source = @"..\..\..\AntMessageSimulatorTests\Device0.txt";
            string destination = @"..\..\..\AntMessageSimulatorTests\Device0.ants";
            string script = "";

            // Clean up previous test run.
            if (File.Exists(destination))
                File.Delete(destination);

            List<DeviceSession> sessions = PowerMeterSimulator.ParseDeviceLog(source);

            // Use the last session to generate a script.
            using (AutoAntsScriptGenerator generator =
                new AutoAntsScriptGenerator(sessions[sessions.Count - 1]))
            {

                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }

            // TODO: Define an actual length to test for here.
            Assert.IsTrue(script.Length > 1000);

            // Write the file out.
            File.WriteAllText(destination, script);
        }
    }
}