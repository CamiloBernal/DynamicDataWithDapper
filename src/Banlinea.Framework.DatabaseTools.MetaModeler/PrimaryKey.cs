namespace Banlinea.Framework.DatabaseTools.MetaModeler
{
    public sealed class PrimaryKey
    {
        public string TableQualifier { get; set; }
        public string TableOwner { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int KeySeq { get; set; }
        public string PkName { get; set; }
    }
}