using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSONToSQL
{
    public static class QueryGenerator
    {
       public static string GenerateQuery(JObject jsonObject, string tableName)
{
    JArray columnsArray = (JArray)jsonObject["columns"];
    
    // Rest of the method remains the same
    // Create a list to store SQL expressions
    List<string> sqlExpressions = new List<string>();

    foreach (var column in columnsArray)
    {
        string? fieldName = column?["fieldName"]?.ToString();
        string? operatorStr = column?["operator"]?.ToString();
        string? fieldValue = column?["fieldValue"]?.ToString();

        // Create SQL expressions based on operators
        string sqlExpression = CreateSQLExpression(fieldName, operatorStr, fieldValue);

        if (!string.IsNullOrEmpty(sqlExpression))
        {
            sqlExpressions.Add(sqlExpression);
        }
    }

    // Combine SQL expressions with "AND" for a complete query
    string finalQuery = string.Join(" AND ", sqlExpressions);

    // Create the final SQL query
    string sqlQuery = $"SELECT * FROM {tableName} WHERE {finalQuery}";
    return sqlQuery;
}



public static string GenerateQueryFromJson(JObject jsonObject)
{
    if (jsonObject == null)
    {
        throw new ArgumentNullException(nameof(jsonObject), "JSON object is null.");
    }

    JArray tablesArray = jsonObject.Value<JArray>("tables");

    if (tablesArray == null || tablesArray.Count == 0)
    {
        throw new ArgumentException("No 'tables' array found in JSON.");
    }

    List<TableDefinition> tableDefinitions = new List<TableDefinition>();

    foreach (var tableJson in tablesArray)
    {
         tableDefinitions.Add(TableDefinition.CreateTableDefinition(tableJson)); // Use the factory method
    }

    StringBuilder queryBuilder = new StringBuilder();

    // Generate the SELECT clause
    string selectClause = GenerateSelectClause(tableDefinitions[0].TableName);
    queryBuilder.Append(selectClause);

    // Generate the JOIN clauses
    string joinClauses = GenerateJoinClauses(tableDefinitions);
    queryBuilder.Append(joinClauses);

    // Generate the WHERE clause
    string whereClause = GenerateWhereClause(tableDefinitions);
    if (!string.IsNullOrEmpty(whereClause))
    {
        queryBuilder.Append(whereClause);
    }

    return queryBuilder.ToString();
}

private static string GenerateSelectClause(string tableName)
{
    return $"SELECT * FROM {tableName}";
}

private static string GenerateJoinClauses(List<TableDefinition> tableDefinitions)
{
    StringBuilder joinClauseBuilder = new StringBuilder();

    foreach (var tableDefinition in tableDefinitions)
    {
        if (!string.IsNullOrEmpty(tableDefinition.JoinType) && !string.IsNullOrEmpty(tableDefinition.JoinCondition))
        {
            joinClauseBuilder.Append($" {tableDefinition.JoinType} {tableDefinition.TableName} ON ({tableDefinition.JoinCondition})");
        }
    }

    return joinClauseBuilder.ToString();
}

private static string GenerateWhereClause(List<TableDefinition> tableDefinitions)
{
    List<string> whereConditions = new List<string>();

    foreach (var tableDefinition in tableDefinitions)
    {
        if (tableDefinition.Conditions.Count > 0)
        {
            whereConditions.AddRange(tableDefinition.Conditions);
        }
    }

    if (whereConditions.Count > 0)
    {
        return " WHERE " + string.Join(" AND ", whereConditions);
    }

    return string.Empty;
}












    public static string CreateSQLExpression(string? fieldName, string? operatorStr, string? fieldValue)
{
    if (operatorStr != null)
    {
        switch (operatorStr.ToUpper())
        {
            case "IN":
                string[] values = fieldValue?.Split(',') ?? new string[0];
                string inValues = string.Join("', '", values);
                return $"{fieldName} IN ('{inValues}')";

            case "=":
                return $"{fieldName} = '{fieldValue}'";

            case "BETWEEN":
                string[] betweenValues = fieldValue?.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                if (betweenValues.Length == 2)
                {
                    return $"{fieldName} BETWEEN {betweenValues[0]} AND {betweenValues[1]}";
                }
                else
                {
                    Console.WriteLine($"Invalid BETWEEN value: {fieldValue}");
                    return string.Empty;
                }

            case "LIKE":
                return $"{fieldName} LIKE '%{fieldValue}%'";

            case "<=":
                return $"{fieldName} <= '{fieldValue}'";

            case ">=":
                return $"{fieldName} >= '{fieldValue}'";

            default:
                Console.WriteLine($"Unsupported operator: {operatorStr}");
                return string.Empty;
        }
    }
    else
    {
        return string.Empty;
    }
}



    }
}
