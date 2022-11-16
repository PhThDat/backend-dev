using BackEndCSharp.Db;
using BackEndCSharp.Model;
using BackEndCSharp.Net;

namespace BackEndCSharp;

class Program
{
    static void Main()
    {
        Console.WriteLine("Server is up");
        List<Listener> listeners = new List<Listener>()
        {
            new HomePageListener(4000),
            new SignInListener(4000),
            new SignUpListener(4000),
            new JavaScriptApiListener(4000),
        };

        foreach (Listener listener in listeners)
            listener.StartAsync();
        
        Console.ReadKey();
    }
}