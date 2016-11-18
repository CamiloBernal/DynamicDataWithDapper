using Banlinea.Framework.DatabaseTools.MetaModeler.Exceptions;
using Banlinea.Framework.DatabaseTools.MetaModeler.Extensions;
using Banlinea.Framework.DatabaseTools.MetaModeler.Helpers;
using Dynamitey;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Builders
{
    public static class ParametersBuilder
    {
        public static async Task<object> BuildInsertParametersAsync(IDbConnection connection, object values, string tableName, string tableSchema = "dbo")
        {
            var columnDefinitions = await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableName, tableSchema).ConfigureAwait(false);
            var writableColumns = columnDefinitions.GetWritableColumns().ToList();
            ValidateRequiredFields(writableColumns, values);
            var parameterMap = new ExpandoObject();
            foreach (var column in writableColumns)
            {
                object value = null;
                if (values.HasMember(column.ColumnName.TrimAll()))
                {
                    value = Dynamic.InvokeGet(values, column.ColumnName.TrimAll());
                }
                if (value != null) Dynamic.InvokeSet(parameterMap, column.ColumnName.TrimAll(), value);
            }
            return parameterMap;
        }

        public static async Task<object> BuildDeleteParametersAsync(IDbConnection connection, object values, string tableName, string tableSchema = "dbo")
        {
            var columnDefinitions = (await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableName, tableSchema).ConfigureAwait(false)).ToList();
            var parameterMap = new ExpandoObject();
            foreach (var column in columnDefinitions)
            {
                object value = null;
                if (values.HasMember(column.ColumnName.TrimAll()))
                {
                    value = Dynamic.InvokeGet(values, column.ColumnName.TrimAll());
                }
                if (value != null) Dynamic.InvokeSet(parameterMap, column.ColumnName.TrimAll(), value);
            }
            return parameterMap;
        }

        public static async Task<object> BuildUpdateParametersAsync(IDbConnection connection, object values, string tableName, string tableSchema = "dbo")
        {
            var columnDefinitions = (await MetadataExtractorHelper.GetTableColumnsAsync(connection, tableName, tableSchema).ConfigureAwait(false)).ToList();
            ValidateRequiredFields(columnDefinitions, values);
            var parameterMap = new ExpandoObject();
            foreach (var column in columnDefinitions)
            {
                object value = null;
                if (values.HasMember(column.ColumnName.TrimAll()))
                {
                    value = Dynamic.InvokeGet(values, column.ColumnName.TrimAll());
                }
                if (value != null) Dynamic.InvokeSet(parameterMap, column.ColumnName.TrimAll(), value);
            }
            return parameterMap;
        }

        private static void ValidateRequiredFields(IEnumerable<ColumnDefinition> columnDefinitions, object values)
        {
            var requiredColumns = columnDefinitions.GetRequiredColumns().Select(c => c.ColumnName).ToList();
            var valueMembers = Dynamic.GetMemberNames(values);
            var missingFields = requiredColumns.Except(valueMembers).ToList();
            if (missingFields.Any()) throw new MissingDataFieldException(missingFields);
        }
    }
}