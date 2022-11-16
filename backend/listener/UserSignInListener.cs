using System.Net;
using BackEndCSharp.Db;
using BackEndCSharp.Model;

namespace BackEndCSharp.Net;

class SignInListener : Listener
{
    private string htmlForm;
    public SignInListener(int port) : base("/user/signin/", port)
        => htmlForm = File.ReadAllText("wwwroot/html/sign-in.html");
    protected override void HandleGET(HttpListenerContext context)
        => Respond(context, htmlForm);
    protected override void HandlePOST(HttpListenerContext context)
    {
        HttpListenerRequest req = context.Request;
        string[] authorizationContent = req.Headers["Authorization"].Split(" ");
        
        byte[] buffer = System.Convert.FromBase64String(authorizationContent[1]);
        string[] namePwPair = System.Text.Encoding.UTF8.GetString(buffer).Split(":");


        if (AccountDb.AuthenticateAccount(namePwPair[0], namePwPair[1]))
        {
            Respond(context, "http://127.0.0.1:4000/");
        }
        else 
        {
            context.Response.StatusCode = 401;
            Respond(context, "Incorrect username or password");
        }
    }
    private string HtmlGenerator(string username)
    {
        return $"<h1>Hello {username}</h1><br><p>I wish to take care of Thu <333</p>";
    }
}