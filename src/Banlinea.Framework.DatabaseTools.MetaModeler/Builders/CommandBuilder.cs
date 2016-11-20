using Banlinea.Framework.DatabaseTools.MetaModeler.Extensions;
using Banlinea.Framework.DatabaseTools.MetaModeler.Helpers;
using Dynamitey;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Builders
{
    public static class CommandBuilder
    {
        public static async Task<string> GetDeleteCommandAsync(TableDefinition tableDefinition, IDbConnection connection)
        {
            if (tableDefinition == null) throw new ArgumentNullException(nameof(tableDefinition));
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(tableDefinition.QualifiedName)) throw new InvalidOperationException("TableNamed is invalid or empty");
            var tableColumns = (await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableDefinition.TableName, tableDefinition.TableSchema).ConfigureAwait(false)).ToList();
            var commandSb = new StringBuilder($"delete from [{tableDefinition.TableSchema}].[{tableDefinition.TableName}] ");
            var where = await GetWhereByFieldsAsync(tableColumns, connection, tableDefinition.TableName, tableDefinition.TableSchema).ConfigureAwait(false);
            if (string.IsNullOrEmpty(where)) throw new InvalidOperationException("Could not recreate the where clause for command");
            commandSb.AppendLine(where);
            return commandSb.ToString();
        }

        public static async Task<string> GetInsertCommandAsync(TableDefinition tableDefinition, IDbConnection connection, bool includeReturnIdentity = false)
        {
            if (tableDefinition == null) throw new ArgumentNullException(nameof(tableDefinition));
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(tableDefinition.QualifiedName)) throw new InvalidOperationException("TableNamed is invalid or empty");

            var tableColumns = await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableDefinition.TableName, tableDefinition.TableSchema).ConfigureAwait(false);
            var columns = tableColumns as IList<ColumnDefinition> ?? tableColumns.ToList();
            var writableColumns = columns.GetWritableColumns();
            var columnDefinitions = writableColumns as IList<ColumnDefinition> ?? writableColumns.ToList();

            var commandSb = new StringBuilder($"insert into [{tableDefinition.TableSchema}].[{tableDefinition.TableName}] (");
            var index = 1;
            foreach (var columnDefinition in columnDefinitions)
            {
                commandSb.AppendLine($" [{columnDefinition.ColumnName}] ");
                if (index < columnDefinitions.Count) commandSb.Append(" , ");
                index++;
            }
            commandSb.Append(") values (");
            index = 1;
            foreach (var columnDefinition in columnDefinitions)
            {
                commandSb.AppendLine($" @{columnDefinition.ColumnName.TrimAll()} ");
                if (index < columnDefinitions.Count) commandSb.Append(" , ");
                index++;
            }
            commandSb.Append(");");

            if (includeReturnIdentity && columns.Any(c => c.IsIdentity))
            {
                commandSb.AppendLine(@"select cast(scope_identity() as bigint);");
            }
            return commandSb.ToString();
        }

        public static async Task<string> GetSelectCommandAsync(IDbConnection connection, string tableName, string tableSchema = "dbo", dynamic filter = null)
        {
            var selectCommand = new StringBuilder($@" Select * from [{tableSchema}].[{tableName}] ");
            var where = await GetWhereByFieldsAsync(connection, tableName, tableSchema, filter).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(where))
            {
                selectCommand.AppendLine(where);
            }
            return selectCommand.ToString();
        }

        public static async Task<string> GetUpdateCommandAsync(TableDefinition tableDefinition, IDbConnection connection)
        {
            if (tableDefinition == null) throw new ArgumentNullException(nameof(tableDefinition));
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(tableDefinition.QualifiedName)) throw new InvalidOperationException("TableNamed is invalid or empty");

            var tableColumns = (await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableDefinition.TableName, tableDefinition.TableSchema).ConfigureAwait(false)).ToList();

            var writableColumns = tableColumns.GetWritableColumns().ToList();

            var commandSb = new StringBuilder($"update [{tableDefinition.TableSchema}].[{tableDefinition.TableName}] set ");

            var index = 1;
            foreach (var column in writableColumns)
            {
                commandSb.AppendLine($" [{column.ColumnName}] = @{column.ColumnName.TrimAll()} ");
                if (index < writableColumns.Count) commandSb.Append(" , ");
                index++;
            }

            var where = await GetWhereByFieldsAsync(tableColumns, connection, tableDefinition.TableName, tableDefinition.TableSchema).ConfigureAwait(false);

            if (string.IsNullOrEmpty(where)) throw new InvalidOperationException("Could not recreate the where clause for command");

            commandSb.AppendLine(where);
            return commandSb.ToString();
        }

        private static async Task<string> GetWhereByFieldsAsync(IDbConnection connection, string tableName, string tableSchema = "dbo", dynamic filter = null)
        {
            if (filter == null) return string.Empty;
            var tableColumns = await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableName, tableSchema).ConfigureAwait(false);

            var columnNames = tableColumns.Select(c => c.ColumnName);
            var filterMembers = (IEnumerable<string>)Dynamic.GetMemberNames(filter);
            var fieldsForWhere = columnNames.Intersect(filterMembers).ToList();
            if (!fieldsForWhere.Any()) return string.Empty;
            var whereSb = new StringBuilder(" where ");
            var index = 1;
            foreach (var field in fieldsForWhere)
            {
                whereSb.AppendLine($@" [{field}] = @{field} ");
                if (index < fieldsForWhere.Count) whereSb.Append(" AND ");
                index++;
            }
            return whereSb.ToString();
        }

        private static async Task<string> GetWhereByFieldsAsync(ICollection<ColumnDefinition> columnDefinitions, IDbConnection connection, string tableName, string tableSchema = "dbo")
        {
            var primaryKeys = (await MetadataExtractorHelper.GetTablePrimaryKeysAsync(connection, tableName, tableSchema).ConfigureAwait(false)).ToList();
            var whereSb = new StringBuilder(" where ");
            var index = 1;
            if (primaryKeys.Any())
            {
                foreach (var pk in primaryKeys)
                {
                    whereSb.AppendLine($"[{pk.ColumnName}] = @{pk.ColumnName.TrimAll()}");
                    if (index < primaryKeys.Count) whereSb.Append(" AND ");
                    index++;
                }
            }
            else
            {
                foreach (var column in columnDefinitions)
                {
                    whereSb.AppendLine($"[{column.ColumnName}] = @{column.ColumnName.TrimAll()}");
                    if (index < columnDefinitions.Count) whereSb.Append(" AND ");
                    index++;
                }
            }
            return whereSb.ToString();
        }
    }
}