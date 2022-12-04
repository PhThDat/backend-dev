const init = async () => {
    let jwt: string = localStorage.getItem("jwt");
    if (jwt == null)
        window.open("http://127.0.0.1:4000/file/html?name=sign-in", "_self");
    else {
        let req: RequestInit = {
            method: "POST",
            headers: [[ "Authorization", `Bearer ${jwt}` ]],
        };

        let response: Response = await fetch("http://127.0.0.1:4000/user/jwt/", req);
        switch (response.status)
        {
            case 401:
                window.open("http://127.0.0.1:4000/user/signin/", "_self");
                break;
            case 200:
                let accountObj = JSON.parse(await response.text());
                let req: RequestInit = {
                    method: "GET",
                    headers: [[ "Authorization", `Bearer ${jwt}`]],
                }
                let res = await fetch(`http://127.0.0.1:4000/user/profile?username=${accountObj.username}`, req);
        }
    }
}

init();