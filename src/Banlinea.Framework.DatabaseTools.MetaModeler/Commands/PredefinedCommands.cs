using System.Collections.Generic;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Commands
{
    public static class PredefinedCommands
    {
        private static readonly IDictionary<string, string> Commands;

        static PredefinedCommands()
        {
            Commands = new Dictionary<string, string>();
            InitializeCommands();
        }

        private static void InitializeCommands()
        {
            Commands["EnumTables"] = @"select s.name [TableSchema] , t.name [TableName] , t.create_date [CreateDate], t.modify_date [ModifyDate]  from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id where t.type = 'U' order by t.name;";
            Commands["GetTableDefinition"] = @"select s.name [TableSchema] , t.name [TableName] , t.create_date [CreateDate], t.modify_date [ModifyDate]  from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id where t.type = 'U' and  s.name = @TableSchema and t.name = @TableName;";
            Commands["GetTablePrimaryKeys"] = @"
                        DECLARE @Results AS TABLE
                          (
                             table_qualifier SYSNAME,
                             table_owner     SYSNAME,
                             table_name      SYSNAME,
                             column_name     SYSNAME,
                             key_seq         SMALLINT,
                             pk_name         SYSNAME
                          )

                        INSERT INTO @Results
                        EXEC Sp_pkeys
                          @table_name = @TableName,
                          @table_owner = @TableSchema;
                        SELECT table_qualifier [TableQualifier],
                               table_owner     [TableOwner],
                               table_name      [TableName],
                               column_name     [ColumnName],
                               key_seq         [KeySeq],
                               pk_name         [PkName]
                        FROM   @Results;";
            Commands["EnumerateColumnsByTable"] = @"
                        select
                        TABLE_CATALOG [TableCatalog],
                        TABLE_SCHEMA [TableSchema],
                        TABLE_NAME [TableName],
                        COLUMN_NAME [ColumnName],
                        ORDINAL_POSITION [OrdinalPosition],
                        COLUMN_DEFAULT [ColumnDefault],
                        case IS_NULLABLE
                        when 'YES' then convert(bit,1) else convert(bit,0) end [IsNullable],
                        DATA_TYPE [DataType],
                        CHARACTER_MAXIMUM_LENGTH [CharacterMaximumLength],
                        CHARACTER_OCTET_LENGTH [CharacterOctetLength],
                        NUMERIC_PRECISION [NumericPrecision],
                        NUMERIC_PRECISION_RADIX [NumericPrecisionRadix],
                        NUMERIC_SCALE [NumericScale],
                        DATETIME_PRECISION [DatetimePrecision],
                        CHARACTER_SET_CATALOG [CharacterSetCatalog],
                        CHARACTER_SET_SCHEMA [CharacterSetSchema],
                        CHARACTER_SET_NAME [CharacterSetName],
                        COLLATION_CATALOG [CollationCatalog],
                        COLLATION_SCHEMA [CollationSchema],
                        COLLATION_NAME [CollationName],
                        DOMAIN_CATALOG [DomainCatalog],
                        DOMAIN_SCHEMA [DomainSchema],
                        DOMAIN_NAME [DomainName],
                        ISNULL(
                        convert(bit,
                        COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA  + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') ),0) [IsIdentity],
                        ISNULL(
                        convert(bit,
                        COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA  + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') ),0) [IsComputed]
                        from information_schema.columns
                        where TABLE_SCHEMA = @TableSchema and TABLE_NAME = @TableName
                        order by ORDINAL_POSITION;
                       ";
        }

        public static string Get(string commandKey)
        {
            string command;

            return Commands.TryGetValue(commandKey, out command) ? command : null;
        }
    }
}