namespace Banlinea.Framework.DatabaseTools.MetaModeler.DatabaseModels
{
    public sealed class ColumnDefinition
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int OrdinalPosition { get; set; }
        public string ColumnDefault { get; set; }
        public bool IsNullable { get; set; }
        public SqlDataType DataType { get; set; }
        public int? CharacterMaximumLength { get; set; }
        public int? CharacterOctetLength { get; set; }
        public int? NumericPrecision { get; set; }
        public int? NumericPrecisionRadix { get; set; }
        public int? NumericScale { get; set; }
        public int? DatetimePrecision { get; set; }
        public string CharacterSetCatalog { get; set; }
        public string CharacterSetSchema { get; set; }
        public string CharacterSetName { get; set; }
        public string CollationCatalog { get; set; }
        public string CollationSchema { get; set; }
        public string CollationName { get; set; }
        public string DomainCatalog { get; set; }
        public string DomainSchema { get; set; }
        public string DomainName { get; set; }

        public bool IsIdentity { get; set; } = false;

        public bool IsComputed { get; set; } = false;

        public bool IsReadOnly => IsIdentity || IsComputed;
    }
}