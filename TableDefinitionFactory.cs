using Newtonsoft.Json.Linq;

namespace JSONToSQL
{
    public class TableDefinitionFactory
    {
        public TableDefinition CreateTableDefinition(JToken tableJson)
        {
              return TableDefinition.CreateTableDefinition(tableJson);
        }
    }
}
