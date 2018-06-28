using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GEOBOX.OSC.IM.DataCheckerTool.Testing.IO
{
    [TestClass]
    public sealed class XmlDataCheckItemsReaderTests
    {
        [TestMethod]
        public void Read_WithTestFile_ReturnsExpectedDataCheckItems()
        {
            var result = XmlTestFileReader.Read();
            Assert.AreEqual(110, result.DataCheckItems.Count());
        }
    }
}
