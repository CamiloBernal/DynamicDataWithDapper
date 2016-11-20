using Banlinea.Framework.DatabaseTools.MetaModeler.Extensions;
using Banlinea.Framework.DatabaseTools.MetaModeler.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dynamitey;


namespace Banlinea.Framework.DatabaseTools.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConfigDatabase"].ConnectionString;
            //IEnumerable<ColumnDefinition> columns = null;

            //using (var connection = new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        columns = MetadataExtractorHelper.GetTableColumnsAsync(connection, "ExtendedListValue", "Process").Result;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(ex.Message);
            //        Console.ForegroundColor = ConsoleColor.Black;
            //    }
            //}


            using (var connection = new SqlConnection(connectionString))
            {
                //Create connection factory
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        Task.WaitAll(connection.OpenAsync());
                    }

                    var data = new { Id = 6};
                    //var result = CrudHelper.InsertAsync(connection,data, "Usuarios", "Test").Result;
                    //var updateTask = CrudHelper.UpdateAsync(connection, data, "Usuarios", "Test");
                    //Task.WaitAll(updateTask);

                    //var deleteTask = CrudHelper.DeleteAsync(connection, data, "Usuarios", "Test");
                    //Task.WaitAll(deleteTask);

                    var result =(IEnumerable<dynamic>) CrudHelper.SelectAsync(connection, "Company", "Process",data).Result;

                    foreach (var r in result)
                    {
                        var fields = Dynamic.GetMemberNames(r);

                        foreach (var f in fields)
                        {
                            var value = Dynamic.InvokeGet(r, f);
                            Console.Write($"{f} : {value}");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine($"Delete ok!!!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToLogString());
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }

            //var columnList = columns as IList<ColumnDefinition> ?? columns.ToList();
            //if (columnList.Any())
            //{
            //    foreach (var column in columnList)
            //    {
            //        //Console.WriteLine($"TableDefinition name: {table.QualifiedName} ,Created at: {table.CreateDate}");
            //        Console.WriteLine($"ColumnDefinition Name: {column.ColumnName} of type: {column.DataType} ");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Cannot list tables");
            //}

            //dynamic data1 = new {A = 1, Name = "Test"};

            //var members = (IEnumerable<string>)Dynamic.GetMemberNames(data);

            //foreach (var member in members)
            //{
            //    Console.WriteLine(member);
            //}

            ////Console.WriteLine(value);

            //dynamic my = new ExpandoObject();

            //Dynamic.InvokeSet(my, "Test", "algo");
            //var value = Dynamic.InvokeGet(my, "Test");
            //Console.WriteLine(value);




            Console.Read();
        }
    }
}