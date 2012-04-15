using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Data.Entity;
using System.Reflection;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Infrastructure;
using Efmap.Bootstrap;
using System.IO;
using System.Xml;

namespace Efmap.Helpers
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Allows buld insert directly onto a table from context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="table"></param>
        /// <param name="data"></param>
        public static void BulkInsert<T>(this DbContext context, string table, IList<T> data)
        {
            BulkInsert<T>(context.Database.Connection.ConnectionString, table, data);
        }


        /// <summary>
        /// allows bulk insert directly onto a specific table
        /// (thanks to Jarod Fergusson, http://elegantcode.com/2012/01/26/sqlbulkcopy-for-generic-listt-useful-for-entity-framework-nhibernate/ )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        public static void BulkInsert<T>(string connection, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))
                    //Dirty hack to make sure we only have system data types 
                    //i.e. filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// call this from your OnModelCreating in your DbContext, it will load every map you 
        /// define for the entities you have in the DbSets attached to this DbContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="modelBuilder"></param>
        public static void AutoLoadForThisContext(this DbContext context, DbModelBuilder modelBuilder)
        {
            //verify options for modelbuilder before
            if (InitializerFactory.IsOptionActivated("RemoveMetaDataStatus"))
            {
                modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            }

            Type tc = context.GetType();
            var props = tc.GetProperties().Where(
                p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)

                ).Select(s => s.PropertyType.GetGenericArguments());

            Assembly asm = Assembly.GetAssembly(context.GetType());
            Type loadType = typeof(EntityTypeConfiguration<>);

            foreach (var dataType in props)
            {
                Type checkType = loadType.MakeGenericType(dataType);
                var findType = asm.GetTypes().Where(t => t.BaseType == checkType).FirstOrDefault();
                if (findType != null)
                {
                    var obj = Activator.CreateInstance(findType);
                    Type registrar = typeof(ConfigurationRegistrar);
                    MethodInfo[] registrarMethods = registrar.GetMethods()
                        .Where(m => m.Name == "Add" && (m.GetGenericArguments().Where(p => p.Name == "TEntityType").Count() == 1)).ToArray();
                    if (registrarMethods.Length == 1)
                    {
                        MethodInfo registrarAdd = registrarMethods[0];
                        MethodInfo genericAdd = registrarAdd.MakeGenericMethod(dataType);

                        var result = genericAdd.Invoke(modelBuilder.Configurations, new[] { obj });
                    }

                }

            }
        }


        public static string GetSqlCreationScript(this DbContext context)
        {
            string entitySqlScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();

            StringBuilder sb = new StringBuilder(entitySqlScript+Environment.NewLine);

            DbInitializer dbinit = InitializerFactory.GetDbInitializerFor(context.GetType());
            SortedSet<string>  customDbCommands = dbinit.GetCommands();
            foreach (string cmd in customDbCommands)
            {
                sb.AppendLine(cmd);
            }
            return sb.ToString();
        }

        public static string GetGeneratedEdmx(this DbContext context)
        {
            string result = string.Empty;
            using (TextWriter writer = new Utf8StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(writer,settings))
                {
                    EdmxWriter.WriteEdmx(context, xmlWriter);
                }
                result = writer.ToString();
            }
            return result;
        }
    }
}
