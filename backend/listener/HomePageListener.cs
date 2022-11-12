using System.Net;

namespace BackEndCSharp.Model;

class HomePageListener : Listener
{
    string htmlForm;
    public HomePageListener(int port) : base("/", port)
        => htmlForm = File.ReadAllText("wwwroot/html/index.html");

    protected override void HandleGET(HttpListenerContext context)
        => Respond(context, htmlForm);
    protected override void HandlePOST(HttpListenerContext context){}
}