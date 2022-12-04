using BackEndCSharp.Model;
using BackEndCSharp.Utility;
using BackEndCSharp.Db;
using System.Net;

namespace BackEndCSharp.Net;

class UserApiListener : Listener
{
    public UserApiListener(int port) : base("/user/", port) {}
    protected override void Init()
    {
        GET("/", handleGET);
        GET("/profile/", handleGETProfile);
        POST("/jwt/", handleJwtPOST);
        POST("/signup/", handleSignUpPOST);
        POST("/signin/", handleSignInPOST);
        PUT("/avatar/", handlePUTAvatar);
    }

    /// <summary>
    /// Handle GET request for default /user/ path. Responses with 401 status code (Unauthorized) on unauthorized client.
    /// </summary>
    private Action<HttpListenerContext> handleGET => (context)
        => Respond(context, File.ReadAllBytes("wwwroot/html/init.html"), HttpStatusCode.OK, "text/html; charset=utf-8");
    /// <summary>
    /// Handle GET request for /user/profile/ path.
    /// </summary>
    private Action<HttpListenerContext> handleGETProfile => (context)
        => Respond(context, File.ReadAllBytes("wwwroot/html/profile.html"), HttpStatusCode.OK, "text/html; charset=utf-8");
    /// <summary>
    /// Authorizing a client's JWT token. Responses 200 status code (OK) on matching, otherwise 401 status code (Unauthorized).
    /// </summary>
    private Action<HttpListenerContext> handleJwtPOST => (context) => {
        HttpListenerRequest request = context.Request;

        string authHeader = request.Headers["Authorization"];
        if (authHeader == null)
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        string[] segments = authHeader.Split(' ');
        if (segments[0] != "Bearer")
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }
        
        (JWTHeader, JWTPayload) headerPayload = JWT.Decode(segments[1]);
        if (!AccountTable.UsernameExists(headerPayload.Item2.Sub) || !AccountTable.JWTTokenMatches(headerPayload.Item2.Sub, segments[1]))
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }
        UserAccount? info = AccountTable.GetInfo(headerPayload.Item2.Sub);
        Respond(context, Json.Stringify(info, nullHandling: JsonStringifyOption.IgnoreNull), HttpStatusCode.OK);
    };
    /// <summary>
    /// Handle a sign up request from client. Responses with a JWT token on success, otherwise a 401 status code (Unauthorized).
    /// </summary>
    private Action<HttpListenerContext> handleSignUpPOST => (context) => {
        dynamic jsonObj = Json.Parse(ReadBody(context.Request));
        UserAccount newAccount = new UserAccount();
        try
        {
            newAccount.Username = jsonObj.username;
            newAccount.Email = jsonObj.email;
            newAccount.Password = jsonObj.password;
        }
        catch
        {
            Respond(context, "", HttpStatusCode.InternalServerError);
            return;
        }
        if (AccountTable.UsernameExists(newAccount.Username))
        {
            Respond(context, "Username not available", HttpStatusCode.Conflict, "text/plain; charset=utf-8");
            return;
        }
        else if (AccountTable.EmailExist(newAccount.Email))
        {
            Respond(context, "Email already taken", HttpStatusCode.Conflict, "text/plain; charset=utf-8");
            return;
        }
        JWT jwt = CreateJWTKey(newAccount);
        Respond(context, Json.Stringify(new { jwt = jwt.ToString() }, nullHandling: JsonStringifyOption.IgnoreNull), HttpStatusCode.Created, "application/json");
    };
    /// <summary>
    /// Handle sign in request from client.Responses with a JWT token on success, otherwise a 401 status code (Unauthorized).
    /// </summary>
    private Action<HttpListenerContext> handleSignInPOST => (context) => {
        HttpListenerRequest request = context.Request;
        string? authHeader = request.Headers["Authorization"];
        if (authHeader == null)
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        string[] segments = authHeader.Split(' ');
        if (segments[0] != "BASIC")
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        string[] unamePwdPair = Converter.FromBase64String(segments[1]).Split(':');
        if (!AccountTable.UsernameExists(unamePwdPair[0]))
        { 
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }
        if (AccountTable.AuthenticateAccount(unamePwdPair[0], unamePwdPair[1]))
        {
            JWT jwt = CreateJWTKey(unamePwdPair[0]);
            Respond(context, Json.Stringify(new { jwt = jwt.ToString() }), HttpStatusCode.OK, "application/json");
        }
        else Respond(context, "", HttpStatusCode.Unauthorized);
    };
    /// <summary>
    /// Handles PUT request for updating or creating user account's avatar
    /// </summary>
    private Action<HttpListenerContext> handlePUTAvatar => (context) => {
        HttpListenerRequest request = context.Request;
        string? authHeader = request.Headers["Authorization"];
        if (authHeader == null)
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        string[] authSegments = authHeader.Split(' ');
        if (authSegments[0] != "Bearer")
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        (JWTHeader, JWTPayload) headerPayload = JWT.Decode(authSegments[1]);
        if (!AccountTable.UsernameExists(headerPayload.Item2.Sub) || !AccountTable.JWTTokenMatches(headerPayload.Item2.Sub, authSegments[1]))
        {
            Respond(context, "", HttpStatusCode.Unauthorized);
            return;
        }

        using MemoryStream memStream = new MemoryStream();
        request.InputStream.CopyTo(memStream);
        bool exists = File.Exists($"wwwroot/images/user/avatar/{headerPayload.Item2.Sub}.png");

        File.WriteAllBytes($"wwwroot/images/user/avatar/{headerPayload.Item2.Sub}.png", memStream.GetBuffer());
        Respond(context, "", exists ? HttpStatusCode.OK : HttpStatusCode.Created, null);
    };
    private JWT CreateJWTKey(UserAccount account)
    {
        byte[] key;
        bool existed;
        do {
            key = JWT.RandomKey();
            existed = AccountTable.JWTKeyExist(key);
        } while (existed);
        JWT jwt = new JWT(
            new JWTHeader()
            {
                Typ = "JWT",
                Alg = "HS256"
            },
            new JWTPayload()
            {
                Iss = "PtDat",
                Sub = account.Username,
                Iat = (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
            }, key);
        AccountTable.AddAccount(account, key);
        return jwt;
    }
    private JWT CreateJWTKey(string username)
    {
        byte[] key;
        bool existed;
        do {
            key = JWT.RandomKey();
            existed = AccountTable.JWTKeyExist(key);
        } while (existed);
        JWT jwt = new JWT(
            new JWTHeader()
            {
                Typ = "JWT",
                Alg = "HS256"
            },
            new JWTPayload()
            {
                Iss = "PtDat",
                Sub = username,
                Iat = (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
            }, key);
        AccountTable.UpdateJWTKey(username, key);
        return jwt;
    }
}