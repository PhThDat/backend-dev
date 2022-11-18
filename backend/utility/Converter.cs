using System.Text;

namespace BackEndCSharp.Utility;

class Converter
{
    public static string FromBase64String(string base64Str)
    {
        byte[] base64EncodedBytes = System.Convert.FromBase64String(base64Str);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
    public static string ToBase64String(string str)
    {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}