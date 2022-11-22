using System.Net;

namespace BackEndCSharp.Net;

class FileApiListener : Listener
{
    public FileApiListener(int port) : base("/file/", port) {}
    protected override void Init()
    {
        GET("/html/", handleGETHtml);
        GET("/js/", handleGETJs);
        GET("/css/", handleGETCss);
    }

    /// <summary>
    /// Responses to a GET request for an HTML file.
    /// </summary>
    private Action<HttpListenerContext> handleGETHtml { get => (context) => {
        string fileName = context.Request.QueryString["name"];
        Respond(context, File.ReadAllBytes($"wwwroot/html/{fileName}.html"), HttpStatusCode.OK, "text/html; charset=utf-8");
    }; }
    /// <summary>
    /// Responses to a GET request for an JavaScript file.
    /// </summary>
    private Action<HttpListenerContext> handleGETJs { get => (context) => {
        string fileName = context.Request.QueryString["name"];
        Respond(context, File.ReadAllBytes($"wwwroot/js/{fileName}.js"), HttpStatusCode.OK, "text/javascript; charset=utf-8");
    }; }
    /// <summary>
    /// Responses to a GET request for an CSS file.
    /// </summary>
    private Action<HttpListenerContext> handleGETCss { get => (context) => {
        string fileName = context.Request.QueryString["name"];
        Respond(context, File.ReadAllBytes($"wwwroot/css/{fileName}.css"), HttpStatusCode.OK, "text/css; charset=utf-8");
    }; }
}