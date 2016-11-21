using Banlinea.Framework.DatabaseTools.MetaModeler.DatabaseModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Banlinea.Framework.DatabaseTools.MetaModeler.Commands;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Helpers
{
    public static class MetadataExtractorHelper
    {
        public static async Task<IEnumerable<TableDefinition>> GetDatabaseTablesAsync(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var command = PredefinedCommands.Get("EnumTables");
            if (string.IsNullOrEmpty(command))
                throw new ConfigurationErrorsException("EnumTables command is not configured.");
            return await connection.QueryAsync<TableDefinition>(command).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<ColumnDefinition>> GetTableColumnsAsync(IDbConnection connection, string tableName, string tableSchema = "dbo")
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            var command = PredefinedCommands.Get("EnumerateColumnsByTable");
            if (string.IsNullOrEmpty(command))
                throw new ConfigurationErrorsException("EnumerateColumnsByTable command is not configured.");

            return await connection.QueryAsync<ColumnDefinition>(command, new { TableSchema = tableSchema, TableName = tableName }).ConfigureAwait(false);
        }

        public static async Task<TableDefinition> GetTableDefinitionAsync(IDbConnection connection, string tableName, string tableSchema = "dbo")
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            var command = PredefinedCommands.Get("GetTableDefinition");
            if (string.IsNullOrEmpty(command))
                throw new ConfigurationErrorsException("GetTableDefinition command is not configured.");
            return await connection.QueryFirstAsync<TableDefinition>(command, new { TableSchema = tableSchema, TableName = tableName }).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<PrimaryKey>> GetTablePrimaryKeysAsync(IDbConnection connection, string tableName, string tableSchema = "dbo")
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            var command = PredefinedCommands.Get("GetTablePrimaryKeys");
            if (string.IsNullOrEmpty(command))
                throw new ConfigurationErrorsException("GetTablePrimaryKeys command is not configured.");
            return await connection.QueryAsync<PrimaryKey>(command, new { TableSchema = tableSchema, TableName = tableName }).ConfigureAwait(false);
        }
    }
}