using JSONToSQL;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class TableDefinition
{
    public string? TableName { get; set; }
    public List<string> Conditions { get; set; }
    public string JoinType { get; set; }
    public string? JoinCondition { get; set; }

    public static TableDefinition CreateTableDefinition(JToken tableJson)
    {
        var tableDefinition = new TableDefinition();
        tableDefinition.TableName = tableJson["name"].ToString();
        tableDefinition.Conditions = new List<string>();
        tableDefinition.JoinType = tableJson["join"]?.ToString() ?? "INNER JOIN";
        tableDefinition.JoinCondition = tableJson["on"]?.ToString();

        JArray conditionsArray = (JArray)tableJson["conditions"];
        foreach (var condition in conditionsArray)
        {
            string fieldName = condition["fieldName"].ToString();
            string operatorStr = condition["operator"].ToString();
            string fieldValue = condition["fieldValue"].ToString();
            string sqlCondition = QueryGenerator.CreateSQLExpression(fieldName, operatorStr, fieldValue);
            if (!string.IsNullOrEmpty(sqlCondition))
            {
                tableDefinition.Conditions.Add(sqlCondition);
            }
        }

        return tableDefinition;
    }
}
