using System.Reflection;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;
using GEOBOX.OSC.IM.DataCheckerTool.IO;

namespace GEOBOX.OSC.IM.DataCheckerTool.Testing.IO
{
    internal sealed class XmlTestFileReader
    {
        public static DataCheckReadResult Read()
        {
            var xmlDataCheckItemsReader = new XmlDataCheckItemsReader();
            var testFileName = "GEOBOX.OSC.IM.DataCheckerTool.Testing.IO.GB_WW_Data_Checker.xml";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testFileName))
            {
                return xmlDataCheckItemsReader.Read(stream, new DataCheckerContext(_ => { }));
            }
        }
    }
}
