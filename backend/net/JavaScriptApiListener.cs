using System.Net;

namespace BackEndCSharp.Net;

class JavaScriptApiListener : Listener
{
    public JavaScriptApiListener(int port) : base("/api/js/", port) {}
    protected override void HandleGET(HttpListenerContext context)
    {
        try
        {
            string jsText = File.ReadAllText("wwwroot/js/" + context.Request.QueryString["file"] + ".js");
            Respond(context, jsText, "text/javascript; charset=UTF-8");
        }
        catch (Exception e)
        {
            context.Response.OutputStream.Close();
        }
    }
    protected override void HandlePOST(HttpListenerContext context) {}
}