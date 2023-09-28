using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JSONToSQL
{
    public class JsonParser
    {
       public static JObject ParseJson(string filePath)
      {
        string jsonInput = File.ReadAllText(filePath);
        return JObject.Parse(jsonInput);
      }
    }
}
