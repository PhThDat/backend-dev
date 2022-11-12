using System.Net;

namespace BackEndCSharp.Model;

class JavaScriptApiListener : Listener
{
    public JavaScriptApiListener(int port) : base("/api/js/", port) {}
    protected override void HandleGET(HttpListenerContext context)
    {
        string jsText = File.ReadAllText("wwwroot/js/" + context.Request.QueryString["file"] + ".js");
        Respond(context, jsText, "text/javascript; charset=UTF-8");
    }
    protected override void HandlePOST(HttpListenerContext context) {}
}