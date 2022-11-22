using BackEndCSharp.Db;
using BackEndCSharp.Model;
using BackEndCSharp.Net;

using NpgsqlTypes;

namespace BackEndCSharp;

class Program
{
    static async Task Main()
    {
        // AccountDb.EnsureCreated();
        List<Listener> listeners = new List<Listener>()
        {
            new RootApiListener(4000),
            new UserApiListener(4000),
            new FileApiListener(4000),
        };

        foreach (Listener listener in listeners)
            listener.StartAsync();
        Console.WriteLine("Server is up");

        Console.ReadKey();
    }
}