const createBtn = (val: string, hrefVal: string, action: () => void = null) => {
    let btn: HTMLInputElement = document.createElement("input");
    btn.type = "button";
    btn.value = val;

    let aHref: HTMLAnchorElement = document.createElement("a");
    aHref.href = hrefVal;
    aHref.addEventListener("click", action);
    aHref.appendChild(btn);

    document.body.appendChild(aHref);
}

const exe = async () => {
    const title: HTMLHeadingElement = document.getElementById("title") as HTMLHeadingElement;
    let jwt: string = localStorage.getItem("jwt");

    if (jwt == null) {
        title.innerHTML = "You are unauthorized";
        createBtn("Sign in", "http://127.0.0.1:4000/file/html?name=sign-in");
    }
    else {
        let req: RequestInit = {
            method: "POST",
            headers: [[ "Authorization", `Bearer ${jwt}` ]]
        };
        let response: Response = await fetch("http://127.0.0.1:4000/user/jwt/", req);

        switch (response.status)
        {
            case 401:
                title.innerHTML = "You are unauthorized";
                createBtn("Sign in", "http://127.0.0.1:4000/file/html?name=sign-in");
                break;
            case 200:
                let payload: string = atob(jwt.split('.')[1]);
                let payloadObj = JSON.parse(payload);
                title.innerHTML = `You are ${payloadObj.sub}`;

                createBtn("Sign out", "http://127.0.0.1:4000/file/html?name=sign-in", () => localStorage.removeItem("jwt"));
        }
    }
}

exe();