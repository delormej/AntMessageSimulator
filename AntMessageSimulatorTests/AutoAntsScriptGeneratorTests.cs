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

            Assert.AreEqual<string>(expected, script);
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

            List<PowerMeterSession> sessions = Program.ParseDeviceLog(source);

            // Use the last session to generate a script.
            using (AutoAntsScriptGenerator generator =
                new AutoAntsScriptGenerator(sessions[sessions.Count - 1]))
            {

                Stream stream = generator.CreateScriptStream();
                TextReader reader = new StreamReader(stream);

                script = reader.ReadToEnd();
            }

            Assert.IsTrue(script.Length == 482273);

            // Write the file out.
            File.WriteAllText(destination, script);
        }
    }
}