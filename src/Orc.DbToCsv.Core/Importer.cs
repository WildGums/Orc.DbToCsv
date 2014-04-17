namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CsvHelper;

    using Orc.DbToCsv.Interfaces;

    public static class Importer
    {
        public static void ProcessProject(Project project, string outputFolder, ILogWriter logWriter)
        {
            logWriter.WriteLine("Project processing started ...");
            using (SqlConnection sqlConnection = new SqlConnection(project.ConnectionString))
            {
                sqlConnection.Open();
                var tables = project.Tables;
                if (project.Tables == null || project.Tables.Count == 0)
                {
                    tables = GetAvailableTables(sqlConnection);
                }

                logWriter.WriteLine(string.Format("{0} tables to process", tables.Count));
                foreach (var tableName in tables)
                {
                    ProcessTable(sqlConnection, tableName, project, outputFolder, logWriter);
                }
            }
        }

        private static void ProcessTable(SqlConnection sqlConnection, string tableName, Project project, string outputFolder, ILogWriter logWriter)
        {
            string pureName = ExtractTableName(tableName);
            string fileName = pureName + ".csv";
            string postfix = (outputFolder[outputFolder.Length - 1] == '\\') ? string.Empty : "\\";
            
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string fullFileName = outputFolder + postfix + fileName;
            int records = 0;

            try
            {
                List<Tuple<string, string>> schema = GetTableSchema(sqlConnection, tableName);
                if (schema.Count == 0)
                {
                    logWriter.WriteLine(string.Format("No columns has found in {0} table to import into csv format", tableName));
                }
                using (StreamWriter streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    CsvWriter csvWriter = new CsvWriter(streamWriter);
                    // Write Header
                    foreach (var tuple in schema)
                    {
                        csvWriter.WriteField(tuple.Item1);
                    }
                    csvWriter.NextRecord();

                    // Write records
                    string query = ConstructRecordQuery(tableName, schema, project.MaximumRowsInTable);
                    using (var command = new SqlCommand(query) {Connection = sqlConnection })
                    {
                        using (var dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                for (int i = 0; i < schema.Count; i++)
                                {
                                    object value = dataReader.GetValue(i);
                                    csvWriter.WriteField(value);
                                }

                                records++;
                                csvWriter.NextRecord();
                            }

                            
                        }
                    }
                }
                string okResult = string.Format(
                "{0} records of {1} table succesfully imported to {2}.",
                records,
                tableName,
                fileName);

                logWriter.WriteLine(okResult);
            }
            catch (Exception ex)
            {
                logWriter.WriteLine(string.Format("{0} import failed because of exception: {1}", tableName, ex.Message));
            }
        }

        private static string ConstructRecordQuery(string tableName, List<Tuple<string, string>> schema, int maximumRowsInTable)
        {
            string columns = string.Join(", ", schema.Select(t => t.Item1));
            string top = maximumRowsInTable > 0 ? "" : string.Empty;

            return string.Format("SELECT {0} {1} FROM {2}", top, columns, tableName);

        }

        private static List<Tuple<string, string>> GetTableSchema(SqlConnection sqlConnection, string tableName)
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT c.name AS Name, t.name AS columnType");
            stringBuilder.AppendLine("FROM sys.columns c INNER JOIN sys.types t ON c.system_type_id = t.system_type_id");
            stringBuilder.AppendFormat(
                " WHERE c.object_id = OBJECT_ID('{0}') and t.name<>'sysname' ",
                tableName);

            string commandText = stringBuilder.ToString();
            using (SqlCommand schemaCommand = new SqlCommand(commandText) { Connection = sqlConnection })
            {
                using (SqlDataReader schemaReader = schemaCommand.ExecuteReader())
                {
                    while (schemaReader.Read())
                    {
                        string name = schemaReader.GetString(0);
                        string stringColumnType = schemaReader.GetString(1);
                        switch (stringColumnType)
                        {
                            case "bit":
                                result.Add(new Tuple<string, string>(name, "boolean"));
                                break;
                            case "varchar":
                            case "nvarchar":
                            case "nchar":
                            case "char":
                                result.Add(new Tuple<string, string>(name, "string"));
                                break;
                            case "int":
                            case "smallint":
                            case "tinyint":
                            case "bigint":
                                result.Add(new Tuple<string, string>(name, "int"));
                                break;
                            case "float":
                            case "decimal":
                                result.Add(new Tuple<string, string>(name, "float"));
                                break;
                            case "datetime":
                                result.Add(new Tuple<string, string>(name, "datetime"));
                                break;
                        }
                    }
                }
            }

            return result;
        }

        private static string ExtractTableName(string tableName)
        {
            int ndx = tableName.LastIndexOf('.');
            return tableName.Substring(ndx + 1).Replace("[", string.Empty).Replace("]", string.Empty);
        }

        private static List<string> GetAvailableTables(SqlConnection sqlConnection)
        {
            const string CommandText = "SELECT '['+TABLE_SCHEMA+'].['+ TABLE_NAME + ']' FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME";

            var result = new List<string>();

            using (SqlCommand schemaCommand = new SqlCommand(CommandText) { Connection = sqlConnection, 
                CommandTimeout = 300 })
            {
                using (SqlDataReader schemaReader = schemaCommand.ExecuteReader())
                {
                    while (schemaReader.Read())
                    {
                        result.Add(schemaReader.GetString(0));
                    }
                }
            }

            return result;
        }
    }
}
