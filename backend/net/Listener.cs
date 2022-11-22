using System.Net;
using System.Diagnostics;

namespace BackEndCSharp.Net;
abstract class Listener
{
    protected string Path;
    private HttpListener listener;
    private Dictionary<string, Action<HttpListenerContext>> actions;
    /// <summary>
    /// Constructor. Give a URL prefix, and a port to which the listener should listen.
    /// </summary>
    public Listener(string path, int port)
    {
        Path = path.TrimEnd('/');
        actions = new Dictionary<string, Action<HttpListenerContext>>();
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:" + port.ToString() + path);
        Init();
    }
    /// <summary>
    /// Start listening to incoming requests asynchronously.
    /// </summary>
    /// <returns>Task indicates the asynchronous state.</returns>
    public async Task StartAsync()
    {
        listener.Start();
        await Task.Delay(1000);

        while (true)
        {
            var result = listener.BeginGetContext(new AsyncCallback((res) => {
                HttpListener l = (HttpListener)res.AsyncState;
                HttpListenerContext context = l.EndGetContext(res);
                HttpListenerRequest request = context.Request;
                try
                {
                    string absPath = request.Url.AbsolutePath;
                    absPath = absPath.EndsWith('/') ? absPath : absPath + '/';
                    actions[$"{request.HttpMethod} {new string(absPath.Skip(Path.Length).ToArray())}"](context);
                }
                catch (Exception exp)
                {
                    Respond(context, "", HttpStatusCode.BadRequest);
                    Console.WriteLine(exp.Message);
                }
                finally 
                {
                    context.Response.Close();
                }
            }), listener);

            result.AsyncWaitHandle.WaitOne();
        }
    }
    /// <summary>
    /// Response to a HTTP context with a string.
    /// </summary>
    protected void Respond(HttpListenerContext context, string msg, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "text/html; charset=utf-8")
    {
        HttpListenerResponse response = context.Response;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);

        response.StatusCode = (int)statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);
        response.OutputStream.Close();
    }
    /// <summary>
    /// Response to a HTTP context with an array of bytes.
    /// </summary>
    protected void Respond(HttpListenerContext context, byte[] stream, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "application/octet-stream")
    {
        HttpListenerResponse response = context.Response;

        response.StatusCode = (int)statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = stream.Length;
        response.OutputStream.Write(stream);
        response.OutputStream.Close();
    }
    /// <summary>
    /// Read the request's body as string.
    /// </summary>
    /// <returns>Decoded string of the body</returns>
    protected string ReadBody(HttpListenerRequest request)
    {
        using StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
        return reader.ReadToEnd();
    }
    /// <summary>
    /// GET action on subPath
    /// </summary>
    protected void GET(string subPath, Action<HttpListenerContext> action)
        => actions.Add($"GET {subPath}", action);
    /// <summary>
    /// POST action on subPath
    /// </summary>
    protected void POST(string subPath, Action<HttpListenerContext> action)
        => actions.Add($"POST {subPath}", action);
    /// <summary>
    /// PUT action on subPath
    /// </summary>
    protected void PUT(string subPath, Action<HttpListenerContext> action)
        => actions.Add($"PUT {subPath}", action);
    /// <summary>
    /// PATCH action on subPath
    /// </summary>
    protected void PATCH(string subPath, Action<HttpListenerContext> action)
        => actions.Add($"PATCH {subPath}", action);
    /// <summary>
    /// DELETE action on subPath
    /// </summary>
    protected void DELETE(string subPath, Action<HttpListenerContext> action)
        => actions.Add($"DELETE {subPath}", action);
    abstract protected void Init();

}