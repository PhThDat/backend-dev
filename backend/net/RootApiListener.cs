using System.Net;

namespace BackEndCSharp.Net;

class RootApiListener : Listener
{
    public RootApiListener(int port) : base("/", port) {}
    protected override void Init()
    {
        GET("/favicon.ico/", (context) => {
            Respond(context, File.ReadAllBytes("wwwroot/images/favicon.ico"), HttpStatusCode.OK, "image/x-icon");
        });
        GET("/", (context) => {
            Respond(context, File.ReadAllBytes("wwwroot/html/index.html"), HttpStatusCode.OK, "text/html; charset=utf-8");
        }); 
    }
}