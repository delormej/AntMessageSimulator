using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class CloudStorageTests
    {
        const string dummyFile = @"..\..\dummy.txt";

        [TestMethod()]
        public void UploadTest()
        {
            CloudStorage storage = new CloudStorage();
            CreateDummyFile(dummyFile);
            storage.Upload(dummyFile);
            File.Delete(dummyFile);
        }

        private void CreateDummyFile(string path)
        {
            File.WriteAllText(path, "dummy sample");
        }
    }
}