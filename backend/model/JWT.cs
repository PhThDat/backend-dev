using System.Security.Cryptography;
using System.Text;

namespace BackEndCSharp.Model;

class JWT
{
    public string Header { get => header; }
    public string Payload { get => payload; }
    public string Signature { get => signture; }
    private string header { get; set; }
    private string payload { get; set; }
    private string signture { get; set; }
    public JWT(string _header, string _payload, string _sign)
    {
        byte[] buffer = System.Convert.FromBase64String(_header);
        header = Encoding.UTF8.GetString(buffer);

        buffer = System.Convert.FromBase64String(_payload);
        payload = Encoding.UTF8.GetString(buffer);
        signture = _sign;

    }
}