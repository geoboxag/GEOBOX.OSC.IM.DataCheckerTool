using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;

namespace GEOBOX.OSC.IM.DataCheckerTool.IO
{
    public sealed class XmlDataCheckItemsWriter
    {
        public void Write(IEnumerable<DataCheckItem> dataCheckItems, XDocument originalDataChecksDocument,
            Stream destinationStream, IDataCheckerContext dataCheckerContext)
        {
            //keeping "other" data in the "original" document
            if (originalDataChecksDocument == null) throw new ArgumentNullException(nameof(originalDataChecksDocument));
            if (destinationStream == null) throw new ArgumentNullException(nameof(destinationStream));
            if (dataCheckerContext == null) throw new ArgumentNullException(nameof(dataCheckerContext));
            Contract.EndContractBlock();

            try
            {
                var dataChecksDocument = new XDocument(originalDataChecksDocument);
                var dataChecksElement = dataChecksDocument.Descendants("DataChecks").FirstOrDefault()
                                                ?? new XElement("DataChecks");
                RemoveDataCheckItems(dataChecksElement);
                ReplaceDataCheckItems(dataCheckItems, dataChecksElement);
                dataChecksDocument.Save(destinationStream);
                dataCheckerContext.LogMessage("Änderungen wurden erfolgreich gespeichert.", TraceLevel.Info);
            }
            catch (Exception exception)
            {
                dataCheckerContext.LogMessage($"Datei konnte nicht gespeichert werden: [{exception.Message}]", TraceLevel.Error);

            }
        }

        public void Write(IEnumerable<DataCheckItem> dataCheckItems, Stream destinationStream)
        {
            //new XDocument from scratch...
            if (destinationStream == null) throw new ArgumentNullException(nameof(destinationStream));
            Contract.EndContractBlock();

            var dataChecksDocument = new XDocument(new XDeclaration("1.0", "utf-8", /*standalone*/null));
            var dataChecksElement = new XElement(XName.Get("DataChecks"));
            ReplaceDataCheckItems(dataCheckItems, dataChecksElement);
            dataChecksDocument.Add(dataChecksElement);
            dataChecksDocument.Save(destinationStream);
        }

        private static void RemoveDataCheckItems(XElement dataChecksElement)
        {
            dataChecksElement.Descendants().Remove();
        }

        private static void ReplaceDataCheckItems(IEnumerable<DataCheckItem> dataCheckItems, XElement dataChecksElement)
        {
            foreach (var dataCheckItem in dataCheckItems)
            {
                dataChecksElement.Add(new XElement(dataCheckItem.Element));
                dataChecksElement.Add(Environment.NewLine);
            }
        }
    }
}
