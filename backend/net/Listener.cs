using System.Net;

namespace BackEndCSharp.Net;

abstract class Listener
{
    private HttpListener listener;
    public string Path;
    public Listener(string path, int port)
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:" + port.ToString() + path);
    }

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

                if (request.HttpMethod == "GET")
                    HandleGET(context);
                else if (request.HttpMethod == "POST")
                    HandlePOST(context);

                context.Response.Close();
            }), listener);

            result.AsyncWaitHandle.WaitOne();
        }
    }
    protected void Respond(HttpListenerContext context, string msg, int statusCode = 200, string contentType = "text/html; charset=UTF-8")
    {
        HttpListenerResponse response = context.Response;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);

        response.StatusCode = statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);
        response.OutputStream.Close();
    }
    protected void Respond(HttpListenerContext context, byte[] stream, int statusCode = 200, string contentType = "application/octet-stream")
    {
        HttpListenerResponse response = context.Response;
        response.StatusCode = statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = stream.Length;
        response.OutputStream.Write(stream);
        response.OutputStream.Close();
    }
    abstract protected void HandleGET(HttpListenerContext context);
    abstract protected void HandlePOST(HttpListenerContext context);
}