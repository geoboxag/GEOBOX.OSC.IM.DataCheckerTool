using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GEOBOX.OSC.IM.DataCheckerTool.Domain
{
    public sealed class DataCheckReadResult
    {
        public string ErrorMessage { get; }

        public IEnumerable<DataCheckItem> DataCheckItems { get; }
        public XDocument DataChecksDocument { get; set; }

        public DataCheckReadResult(XDocument dataChecksDocument, IEnumerable<DataCheckItem> dataCheckItems)
        {
            DataCheckItems = dataCheckItems;
            DataChecksDocument = dataChecksDocument;
        }

        public DataCheckReadResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool HasErrorMessage()
        {
            return !String.IsNullOrEmpty(ErrorMessage);
        }

    }
}
