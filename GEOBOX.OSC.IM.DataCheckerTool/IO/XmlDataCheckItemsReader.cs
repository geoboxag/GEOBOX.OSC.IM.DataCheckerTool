using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;

namespace GEOBOX.OSC.IM.DataCheckerTool.IO
{
    public sealed class XmlDataCheckItemsReader
    {
        public DataCheckReadResult Read(Stream dataChecksXmlStream, IDataCheckerContext dataCheckerContext)
        {
            if (dataChecksXmlStream == null) throw new ArgumentNullException(nameof(dataChecksXmlStream));
            if (dataCheckerContext == null) throw new ArgumentNullException(nameof(dataCheckerContext));
            Contract.EndContractBlock();

            return TryRead
                (
                () =>
                    {
                        var dataChecksDocument = XDocument.Load(dataChecksXmlStream);
                        var dataCheckItems =
                            from dataCheckElement
                            in dataChecksDocument.Descendants("DataCheck")
                            select new DataCheckItem(dataCheckElement);

                        return new DataCheckReadResult(dataChecksDocument, dataCheckItems);
                    }, dataCheckerContext
                );
        }

        private DataCheckReadResult TryRead(Func<DataCheckReadResult> toExecuteAction, IDataCheckerContext dataCheckerContext)
        {
            try
            {
                return toExecuteAction();
            }
            catch (Exception exception)
            {
                var errorMessage = $"Datei kann nicht gelesen werden: [{exception.Message}]";
                dataCheckerContext.LogMessage(errorMessage, TraceLevel.Error);
                return new DataCheckReadResult(errorMessage);
            }
        }

        public DataCheckReadResult Read(string fullFileName, IDataCheckerContext dataCheckerContext)
        {
            return TryRead(() =>
            {
                using (var fileStream = new FileStream(fullFileName, FileMode.Open))
                {
                    return Read(fileStream, dataCheckerContext);
                }
            }, dataCheckerContext);
        }
    }
}
