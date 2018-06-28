using System.IO;
using System.Linq;
using GEOBOX.IM.CH.DataChecker.Core.Domain;
using GEOBOX.IM.CH.DataChecker.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GEOBOX.IM.CH.DataChecker.Core.Testing.IO
{
    [TestClass]
    public sealed  class XmlDataCheckItemsWriterTests
    {
        [TestMethod]
        public void Write_WithTestData_ReturnsExpectedResult()
        {
            var result = XmlTestFileReader.Read();
            var writer = new XmlDataCheckItemsWriter();
            using (var testMemoryStream = new MemoryStream())
            {
                writer.Write(result.DataCheckItems, result.DataChecksDocument, testMemoryStream, new DataCheckerContext(/*logMessageAction*/null));
                var reader = new XmlDataCheckItemsReader();
                testMemoryStream.Position = 0; //reset position to start!
                var readerResultAfterWrite = reader.Read(testMemoryStream, new DataCheckerContext(/*logMessageAction*/null));
                //we just assume that the count of DataCheckItems is the same
                Assert.AreEqual(result.DataCheckItems.Count(), readerResultAfterWrite.DataCheckItems.Count());
                //Debug.WriteLine(Encoding.UTF8.GetString(testMemoryStream.ToArray()));
            }

        }
    }
}
