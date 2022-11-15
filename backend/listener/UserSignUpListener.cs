using System.Net;
using Newtonsoft.Json;
using BackEndCSharp.Db;

namespace BackEndCSharp.Model;

class SignUpListener : Listener
{
    private AccountDb accDb;
    private string htmlForm;
    public SignUpListener(int port) : base("/user/signup/", port)
    {
        accDb = new AccountDb();
        htmlForm = File.ReadAllText("wwwroot/html/sign-up.html");
    }
    protected override void HandleGET(HttpListenerContext context)
        => Respond(context, htmlForm);
    protected override void HandlePOST(HttpListenerContext context)
    {
        HttpListenerRequest req = context.Request;
        StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);
        string body = reader.ReadToEnd();
        UserAccount newAccount = JsonConvert.DeserializeObject<UserAccount>(body);

        if (accDb.UsernameExists(newAccount.Username))
            Respond(context, "Username not available");
        else
        {
            accDb.AddAccount(newAccount);
            Respond(context, "Signed up successfully");
        }

        reader.Close();
        req.InputStream.Close();
    }
}