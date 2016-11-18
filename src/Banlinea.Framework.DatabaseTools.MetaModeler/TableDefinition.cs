using System;

namespace Banlinea.Framework.DatabaseTools.MetaModeler
{
    public class TableDefinition
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string QualifiedName => $"{TableSchema}.{TableName}";
    }
}