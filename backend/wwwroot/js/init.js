(async () => {
    var jwt = localStorage.getItem("jwt");

    if (jwt == null) {
        window.open("http://127.0.0.1:4000/file/html?name=sign-in", "_self");
    }
    else {
        var req = {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${jwt}`,
            }
        }
        var response = await fetch("http://127.0.0.1:4000/user/jwt/", req);
        if (response.status == 401)
            window.open("http://127.0.0.1:4000/file/html?name=sign-in", "_self");
        else if (response.status == 200)
            window.open("http://127.0.0.1:4000/file/html?name=profile", "_self");
    }
})(); 