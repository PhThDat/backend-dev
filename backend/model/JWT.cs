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
    public JWT(string _header, string _payload, byte[] key)
    {
        header = Converter.ToBase64String(_header);
        payload = Converter.ToBase64String(_payload);
        using HMACSHA256 hmac = new HMACSHA256(key);
        byte[] sign = hmac.ComputeHash(Encoding.UTF8.GetBytes(header + '.' + payload));
        signture = System.Convert.ToBase64String(sign);
    }
    public bool Verify(byte[] key)
    {
        using HMACSHA256 hmac = new HMACSHA256(key);
        byte[] s = hmac.ComputeHash(Encoding.UTF8.GetBytes(header + '.' + payload));
        string sign = System.Convert.ToBase64String(s);
        return sign == Signature;
    }
    public override string ToString() => Header + '.' + Payload + '.' + Signature;
    public static string[] Decode(string jwt)
    {
        string[] segments = jwt.Split('.');
        string[] headPayload = new string[2];
        headPayload[0] = Converter.FromBase64String(segments[0]);
        headPayload[1] = Converter.FromBase64String(segments[1]);
        return headPayload;
    }
    public static byte[] RandomKey()
    {
        byte[] key = new byte[32];
        Random rand = new Random();
        for (int i = 0; i < 32; i++)
            key[i] = Convert.ToByte(rand.Next(0, 256));
        
        return key;
    }
}