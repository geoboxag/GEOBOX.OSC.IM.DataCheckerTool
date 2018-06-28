using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace GEOBOX.OSC.IM.DataCheckerTool.Domain
{
    public sealed class DataCheckItem
    {
        public XElement Element { get; }

        public int Id { get; }

        public int ParentDataCheckId { get; }

        public string Name { get; }
        public string SqlStatement { get; }

        public bool IsSqlCheckItem { get; }

        public bool SupportsSortAll { get; }

        public string Description { get; }

        public DataCheckItem(XElement xElement)
        {
            if (xElement == null) throw new ArgumentNullException(nameof(xElement));
            Contract.EndContractBlock();

            Element = xElement;

            Id = int.Parse(xElement.Attributes(GetIdXName()).First().Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            ParentDataCheckId = int.Parse(xElement.Attributes(GetParentDataCheckIdXName()).First().Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            Name = xElement.Attributes(GetNameXName()).First().Value;
            var sqlStatement = xElement.Attributes(GetDataCheckSqlStatementXName()).FirstOrDefault()?.Value;
            IsSqlCheckItem = IsValidSqlStatement(sqlStatement);
            SqlStatement = sqlStatement;
            Description = xElement.Attributes(GetDescriptionXName()).FirstOrDefault()?.Value;
        }

        private static bool IsValidSqlStatement(string sqlStatement)
        {
            return !String.IsNullOrEmpty(sqlStatement);
        }

        public DataCheckItem(int id, string name, bool isSqlCheckItem)
        {
            Id = id;
            Name = name;
            IsSqlCheckItem = isSqlCheckItem;
        }

        public DataCheckItem(int id, string name, bool isSqlCheckItem, bool supportsSortAll):this(id, name, isSqlCheckItem)
        {
            SupportsSortAll = supportsSortAll;
        }

        /// <summary>
        /// Creates a deep copy of dataCheckItem.
        /// </summary>
        /// <param name="dataCheckItem"></param>
        public DataCheckItem(DataCheckItem dataCheckItem)
        {
            Element = new XElement(dataCheckItem.Element);
            Id = dataCheckItem.Id;
            ParentDataCheckId = dataCheckItem.ParentDataCheckId;
            Name = dataCheckItem.Name;
            IsSqlCheckItem = dataCheckItem.IsSqlCheckItem;
            SqlStatement = dataCheckItem.SqlStatement;
        }

        private static XName GetDataCheckSqlStatementXName()
        {
            return XName.Get("DataCheckSqlStatement");
        }

        private static XName GetNameXName()
        {
            return XName.Get("Name");
        }

        private static XName GetParentDataCheckIdXName()
        {
            return XName.Get("ParentDataCheckID");
        }

        private static XName GetIdXName()
        {
            return XName.Get("ID");
        }

        private static XName GetDescriptionXName()
        {
            return XName.Get("Description");
        }

        public DataCheckItem CorrectPosition(int newId, int newParentDataCheckId)
        {
            var copyXElement = new XElement(Element);
            copyXElement.Attributes(GetIdXName()).First().SetValue(newId);
            copyXElement.Attributes(GetParentDataCheckIdXName()).First().SetValue(newParentDataCheckId);
            return new DataCheckItem(copyXElement);
        }
    }
}
