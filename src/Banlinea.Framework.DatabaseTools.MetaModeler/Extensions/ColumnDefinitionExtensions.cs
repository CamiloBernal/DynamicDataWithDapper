using System.Collections.Generic;
using System.Linq;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Extensions
{
    public static class ColumnDefinitionExtensions
    {
        public static IEnumerable<ColumnDefinition> GetWritableColumns(this IEnumerable<ColumnDefinition> columnDefinitions) => columnDefinitions.Where(cd => !cd.IsReadOnly).ToList().AsParallel();

        public static IEnumerable<ColumnDefinition> GetRequiredColumns(this IEnumerable<ColumnDefinition> columnDefinitions) => columnDefinitions.Where(cd => !cd.IsNullable).ToList().AsParallel();
    }
}