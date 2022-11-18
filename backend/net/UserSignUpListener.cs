using System.Net;
using Newtonsoft.Json;
using BackEndCSharp.Db;
using BackEndCSharp.Model;

namespace BackEndCSharp.Net;

class SignUpListener : Listener
{
    private string htmlForm;
    public SignUpListener(int port) : base("/user/signup/", port)
        => htmlForm = File.ReadAllText("wwwroot/html/sign-up.html");
    protected override void HandleGET(HttpListenerContext context)
        => Respond(context, htmlForm);
    protected override void HandlePOST(HttpListenerContext context)
    {
        HttpListenerRequest req = context.Request;
        StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);
        string body = reader.ReadToEnd();
        UserAccount newAccount = JsonConvert.DeserializeObject<UserAccount>(body);

        if (AccountDb.UsernameExists(newAccount.Username))
        {
            Respond(context, "Username not available", 409);
        }
        else
        {
            bool successful;
            byte[] jwtKey = null;
            AccountDb.AddAccount(newAccount);
            do {
                jwtKey = JWT.RandomKey();
                successful = AccountDb.AddJWTKey(newAccount.Username, jwtKey);
            } while (!successful);
            Respond(context, jwtKey, 201);
        }

        reader.Close();
        req.InputStream.Close();
    }
}