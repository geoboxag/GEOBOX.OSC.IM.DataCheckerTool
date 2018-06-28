using System.Reflection;
using GEOBOX.IM.CH.DataChecker.Core.Domain;
using GEOBOX.IM.CH.DataChecker.Core.IO;

namespace GEOBOX.IM.CH.DataChecker.Core.Testing.IO
{
    internal sealed class XmlTestFileReader
    {
        public static DataCheckReadResult Read()
        {
            var xmlDataCheckItemsReader = new XmlDataCheckItemsReader();
            var testFileName = "GEOBOX.IM.CH.DataChecker.Core.Testing.IO.GB_WW_Data_Checker.xml";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testFileName))
            {
                return xmlDataCheckItemsReader.Read(stream, new DataCheckerContext(_ => { }));
            }
        }
    }
}
