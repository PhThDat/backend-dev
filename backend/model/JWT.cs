using System.Security.Cryptography;
using System.Text;
using BackEndCSharp.Utility;

namespace BackEndCSharp.Model;

class JWT
{
    public string Header { get => header; }
    public string Payload { get => payload; }
    public string Signature { get => signture; }
    private string header { get; set; }
    private string payload { get; set; }
    private string signture { get; set; }
    public JWT(string _header, string _payload, string key)
    {
        header = Converter.ToBase64String(_header);
        payload = Converter.ToBase64String(_payload);
        using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] sign = hmac.ComputeHash(Encoding.UTF8.GetBytes(header + '.' + payload));
        signture = Encoding.UTF8.GetString(sign);
    }
    public bool Verify(string key)
    {
        using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] s = hmac.ComputeHash(Encoding.UTF8.GetBytes(header + '.' + payload));
        string sign = Encoding.UTF8.GetString(sign);
        return sign == signature;
    }
    public string ToString() => Header + '.' + Payload + '.' + Signature;
    //0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz
    //8Hvdzt1nYyMZrpUuoEDs347x5hGOLTg2jBRJISXfk6Km9WPQeqalwcNiAbF0CV
}