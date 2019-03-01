using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nier.Redis.Tests
{
    [TestClass]
    public class AssemblyResourceReaderTests
    {
        private AssemblyResourceReader _reader = new AssemblyResourceReader(Assembly.GetExecutingAssembly());

        [TestMethod]
        public void ReadFile_PassAbsolutePath_Success()
        {
            Assert.AreEqual("Drop that pickle!", _reader.ReadFile("Nier.Redis.Tests.testFile.txt"));
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadFile_PassAbsolutePathOfNoneExistFile_Fail()
        {
            _reader.ReadFile("Nier.Redis.Tests.NotExist.txt");
        }

        [TestMethod]
        public void ReadFile_PassRelativePath_Success()
        {
            Assert.AreEqual("Drop that pickle!", _reader.ReadFile(typeof(AssemblyResourceReaderTests), "testFile.txt"));
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadFile_PassRelativePath_Fail()
        {
            _reader.ReadFile(typeof(AssemblyResourceReaderTests), "notExist.txt");
        }
    }
}