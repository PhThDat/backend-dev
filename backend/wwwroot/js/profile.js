const createBtn = (val, hrefVal, event = null) => {
    var btn = document.createElement("input");
    btn.type = "button";
    btn.value = val;

    var aHref = document.createElement("a");
    aHref.href = hrefVal;
    aHref.addEventListener("click", event);
    aHref.appendChild(btn);

    document.body.appendChild(aHref);
}

(async () => {
    const title = document.getElementById("title");
    var jwt = localStorage.getItem("jwt");

    if (jwt == null)
    {
        title.innerHTML = "You are unauthorized";
        createBtn("Sign in", "http://127.0.0.1:4000/file/html?name=sign-in");
    }
    else {
        var req = {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${jwt}`,
            }
        };
        var response = await fetch("http://127.0.0.1:4000/user/jwt/", req);
        if (response.status == 401)
        {
            title.innerHTML = "You are unauthorized";
            createBtn("Sign in", "http://127.0.0.1:4000/file/html?name=sign-in");
        }
        else if (response.status == 200) {
            var payload = atob(jwt.split('.')[1]);
            var payloadObj = JSON.parse(payload);
            title.innerHTML = `You are ${payloadObj.sub}`;

            createBtn("Sign out", "http://127.0.0.1:4000/file/html?name=sign-in", () => localStorage.removeItem("jwt"));
        }
    }
})();