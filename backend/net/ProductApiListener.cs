using System.Net;
using BackEndCSharp.Model;
using BackEndCSharp.Utility;
using BackEndCSharp.Db;

namespace BackEndCSharp.Net;

class ProductApiListener : Listener
{
    public ProductApiListener(int port) : base("/product/", port) {}
    protected override void Init()
    {
        GET("/goods/", handleGETGoods);
    }

    private Action<HttpListenerContext> handleGETGoods => (context) => {
        HttpListenerRequest request = context.Request;

        string? keyword = request.QueryString["keyword"];
        int? amt = Int32.Parse(request.QueryString["amt"]);
        if (amt != null && keyword == "*")
        {
            GoodsInfo[] gInfos = GoodsTable.GetRandomGoods((int)amt);
            string json = Json.Stringify(gInfos);
            Respond(context, json, HttpStatusCode.OK, "application/json");
        }
    };
}