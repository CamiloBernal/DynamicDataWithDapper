using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Helpers
{
    public static class CrudHelper
    {
        public static async Task<long?> InsertAsync(IDbConnection connection, dynamic values, string tableName, string tableSchema = "dbo", bool retrieveIdentityValue = false)
        {
            var tableDefinition = await MetadataExtractorHelper.GetTableDefinitionAsync(connection, tableName, tableSchema).ConfigureAwait(false);
            var buildCommandTask = Builders.CommandBuilder.GetInsertCommandAsync(tableDefinition, connection, retrieveIdentityValue);
            var buildParametersTask = Builders.ParametersBuilder.BuildInsertParametersAsync(connection, values, tableName, tableSchema);
            await Task.WhenAll(buildCommandTask, buildParametersTask).ConfigureAwait(false);
            var command = await buildCommandTask.ConfigureAwait(false);
            var parameters = await buildParametersTask;

            if (retrieveIdentityValue)
            {
                return await connection.QueryFirstAsync<int>(command, (object)parameters).ConfigureAwait(false);
            }
            await connection.ExecuteAsync(command, (object)parameters).ConfigureAwait(false);
            return -1;
        }

        public static async Task UpdateAsync(IDbConnection connection, dynamic values, string tableName, string tableSchema = "dbo")
        {
            var tableDefinition = await MetadataExtractorHelper.GetTableDefinitionAsync(connection, tableName, tableSchema).ConfigureAwait(false);
            var buildCommandTask = Builders.CommandBuilder.GetUpdateCommandAsync(tableDefinition, connection);
            var buildParametersTask = Builders.ParametersBuilder.BuildUpdateParametersAsync(connection, values, tableName, tableSchema);
            await Task.WhenAll(buildCommandTask, buildParametersTask).ConfigureAwait(false);
            var command = await buildCommandTask.ConfigureAwait(false);
            var parameters = await buildParametersTask;
            await connection.ExecuteAsync(command, (object)parameters).ConfigureAwait(false);
        }

        public static async Task DeleteAsync(IDbConnection connection, dynamic values, string tableName, string tableSchema = "dbo")
        {
            var tableDefinition = await MetadataExtractorHelper.GetTableDefinitionAsync(connection, tableName, tableSchema).ConfigureAwait(false);
            var buildCommandTask = Builders.CommandBuilder.GetDeleteCommandAsync(tableDefinition, connection);
            var buildParametersTask = Builders.ParametersBuilder.BuildDeleteParametersAsync(connection, values, tableName, tableSchema);
            await Task.WhenAll(buildCommandTask, buildParametersTask).ConfigureAwait(false);
            var command = await buildCommandTask.ConfigureAwait(false);
            var parameters = await buildParametersTask.ConfigureAwait(false);
            await connection.ExecuteAsync(command, (object)parameters).ConfigureAwait(false);
        }

        public static async Task<dynamic> SelectAsync(IDbConnection connection, string tableName, string tableSchema = "dbo", dynamic filter = null)
        {
            var buildCommandTask = Builders.CommandBuilder.GetSelectCommandAsync(connection, tableName, tableSchema, filter);
            var buildParametersTask = Builders.ParametersBuilder.BuildSelectParametersAsync(connection, filter, tableName, tableSchema);
            await Task.WhenAll(buildCommandTask, buildParametersTask);
            var command = await buildCommandTask.ConfigureAwait(false);
            var parameters = await buildParametersTask.ConfigureAwait(false);
            return await connection.QueryAsync<dynamic>((string)command, (object)parameters).ConfigureAwait(false);
        }
    }
}