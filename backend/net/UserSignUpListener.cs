using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using BackEndCSharp.Db;
using BackEndCSharp.Model;

namespace BackEndCSharp.Net;

class SignUpListener : Listener
{
    private string htmlForm;
    public SignUpListener(int port) : base("/user/signup/", port)
        => htmlForm = File.ReadAllText("wwwroot/html/sign-up.html");
    protected override void HandleGET(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;

        if (request.Headers["Authorization"] == null)
            Respond(context, htmlForm);
    }
    protected override void HandlePOST(HttpListenerContext context)
    {   
        HttpListenerRequest req = context.Request;
        StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);
        string body = reader.ReadToEnd();
        UserAccount newAccount = JsonConvert.DeserializeObject<UserAccount>(body);

        if (AccountDb.UsernameExists(newAccount.Username))
        {
            Respond(context, "Username not available", 409);
        }
        else
        {
            AccountDb.AddAccount(newAccount);
            JWT jwt = CreateUserJWT(newAccount);
            Respond(context, null, 201, CreateCookies(jwt));
        }

        reader.Close();
        req.InputStream.Close();
    }
    private JWT CreateUserJWT(UserAccount account)
    {
        bool successful;
        byte[] jwtKey = null;
        JWTHeader jwtHeader = new JWTHeader()
        {
            Typ = "JWT",
            Alg = "HS256"
        };
        JWTPayload jWTPayload = new JWTPayload()
        {
            Iss = "PtDat",
            Sub = account.Username,
            Iat = Convert.ToUInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)
        };
        
        do {
            jwtKey = JWT.RandomKey();
            successful = AccountDb.AddJWTKey(account.Username, jwtKey);
        } while (!successful);

        return new JWT(jwtHeader, jWTPayload, jwtKey);
    }
    ICollection<Cookie> CreateCookies(JWT jwt)
    {
        return new Cookie[] {
            new Cookie("jwt", jwt.ToString())
        };
    }
}