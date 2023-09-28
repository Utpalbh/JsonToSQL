using System;
using Newtonsoft.Json.Linq;

namespace JSONToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Parse the JSON using the JsonParser class
              JObject columnsObject = JsonParser.ParseJson("data1.json");

                // Generate the SQL query using the QueryGenerator class
                string tableName = "table1"; 
                string sqlQuery = QueryGenerator.GenerateQuery(columnsObject, tableName);

                // Print the final SQL query
                Console.WriteLine("Generated SQL Query:");
                Console.WriteLine(sqlQuery);

                string jsonInput = File.ReadAllText("data2.json");
                JObject jsonObject = JObject.Parse(jsonInput);

                string finalQuery = QueryGenerator.GenerateQueryFromJson(jsonObject);

                    // Print the final SQL query
                Console.WriteLine("Generated SQL Query using inner join:");
                Console.WriteLine(finalQuery);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
