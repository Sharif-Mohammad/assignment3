using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CJTPWebserver
{
    public static class JsonUtil
    {
        public static T Deserialize<T>(string requestString) where T : class
        {
            return JsonSerializer.Deserialize<T>(requestString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public static string Serialize<T>(T requestString) where T : class
        {
            return JsonSerializer.Serialize(requestString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public static bool IsValidJson(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true; // Valid JSON
            }
            catch (JsonException)
            {
                return false; // Invalid JSON
            }
            catch (ArgumentNullException)
            {
                return false; // Null or empty string
            }
        }

    }
}
