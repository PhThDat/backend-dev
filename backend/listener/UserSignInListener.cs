using System.Net;

namespace BackEndCSharp.Model;

class SignInListener : Listener
{
    private string htmlForm;
    public SignInListener(int port) : base("/user/signin/", port)
        => htmlForm = File.ReadAllText("wwwroot/html/sign-in.html");
    protected override void HandleGET(HttpListenerContext context)
        => Respond(context, htmlForm);
    protected override void HandlePOST(HttpListenerContext context) {}
}