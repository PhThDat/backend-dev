using Newtonsoft.Json;

namespace BackEndCSharp.Utility;

static class Json
{
    /// <summary>
    /// Parse an object to camelCase JSON
    /// </summary>
    /// <returns>camelCase JSON</returns>
    public static string Stringify(object obj)
        => JsonConvert.SerializeObject(obj,
            new JsonSerializerSettings { 
                ContractResolver = new Newtonsoft.Json.Serialization.
                                    CamelCasePropertyNamesContractResolver(),
            });
    /// <summary>
    /// Parse a JSON.
    /// </summary>
    /// <returns>An object of anonymous type represents the original JSON.</returns>
    public static dynamic Parse(string json)
        => JsonConvert.DeserializeObject(json);
} 