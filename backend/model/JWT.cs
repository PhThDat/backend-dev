using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using BackEndCSharp.Utility;

namespace BackEndCSharp.Model;

class JWT
{
    public JWTHeader Header { get => header; }
    public JWTPayload Payload { get => payload; }
    public string Signature { get => stringForm[2]; }
    private JWTHeader header;
    private JWTPayload payload;
    private string[] stringForm;
    public JWT(string _header, string _payload, byte[] key)
    {
        header = JsonConvert.DeserializeObject<JWTHeader>(_header);
        payload = JsonConvert.DeserializeObject<JWTPayload>(_payload);

        stringForm = new string[3] { Header.Base64Encoded(), Payload.Base64Encoded(), null};
        using HMACSHA256 hmac = new HMACSHA256(key);
        byte[] sign = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringForm[0] + '.' + stringForm[1]));
        stringForm[2] = System.Convert.ToBase64String(sign);
    }
    public JWT(JWTHeader _header, JWTPayload _payload, byte[] key)
    {
        header = _header;
        payload = _payload;

        stringForm = new string[3] { Header.Base64Encoded(), Payload.Base64Encoded(), null};
        using HMACSHA256 hmac = new HMACSHA256(key);
        byte[] sign = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringForm[0] + '.' + stringForm[1]));
        stringForm[2] = System.Convert.ToBase64String(sign);
    }
    public bool Verify(byte[] key)
    {
        using HMACSHA256 hmac = new HMACSHA256(key);
        byte[] s = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringForm[0] + '.' + stringForm[1]));
        string sign = System.Convert.ToBase64String(s);
        return sign == stringForm[2];
    }
    public override string ToString() => String.Join('.', stringForm);
    public static (JWTHeader, JWTPayload) Decode(string jwt)
    {
        string[] segments = jwt.Split('.');
        segments[0] = Converter.FromBase64String(segments[0]);
        segments[1] = Converter.FromBase64String(segments[1]);
        return (JsonConvert.DeserializeObject<JWTHeader>(segments[0]),
                JsonConvert.DeserializeObject<JWTPayload>(segments[1]));
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

class JWTHeader
{
    public string Typ;
    public string Alg;
    public override string ToString()
        => JsonConvert.SerializeObject(this,
        new JsonSerializerSettings { 
            ContractResolver = new Newtonsoft.Json.Serialization.
                                   CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        });
    public string Base64Encoded()
        => Converter.ToBase64String(ToString());
}

class JWTPayload
{
    public string? Iss;
    public string? Sub;
    public string? Aud;
    public ulong? Exp;
    public ulong? Nbf;
    public ulong? Iat;
    public string? Jti;
    public override string ToString()
        => JsonConvert.SerializeObject(this,
        new JsonSerializerSettings { 
            ContractResolver = new Newtonsoft.Json.Serialization.
                                   CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        });
    public string Base64Encoded()
        => Converter.ToBase64String(ToString());
}