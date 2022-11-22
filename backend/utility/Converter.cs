using System.Text;
using Newtonsoft.Json;

namespace BackEndCSharp.Utility;

static class Converter
{
    /// <summary>
    /// Decodes from base64 string to string.
    /// </summary>
    /// <returns>Decoded string.</returns>
    public static string FromBase64String(string base64Str)
    {
        byte[] base64EncodedBytes = System.Convert.FromBase64String(base64Str);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
    /// <summary>
    /// Encoding a string to base64 one
    /// </summary>
    /// <returns>Encoded base64 string</returns>
    public static string ToBase64String(string str)
    {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}