using Newtonsoft.Json;

namespace BackEndCSharp.Utility;

enum JsonStringifyOption
{
    PascalCase,
    CamelCase,
    IgnoreNull,
    AcceptNull,
}

static class Json
{
    /// <summary>
    /// Parse an object to camelCase JSON
    /// </summary>
    /// <returns>camelCase JSON</returns>
    public static string Stringify(object obj,
                                JsonStringifyOption caseFormat = JsonStringifyOption.CamelCase,
                                JsonStringifyOption nullHandling = JsonStringifyOption.AcceptNull)
    {
        Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver? resolver = null;
        switch (caseFormat)
        {
            case JsonStringifyOption.CamelCase:
                resolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                break;
        }

        NullValueHandling handler = (nullHandling == JsonStringifyOption.IgnoreNull) ? NullValueHandling.Ignore : NullValueHandling.Include;
        return JsonConvert.SerializeObject(obj,
            new JsonSerializerSettings { 
                ContractResolver = resolver,
                NullValueHandling = handler,
            });
    }
    /// <summary>
    /// Parse a JSON.
    /// </summary>
    /// <returns>An object of anonymous type represents the original JSON.</returns>
    public static dynamic Parse(string json)
        => JsonConvert.DeserializeObject(json);
} 