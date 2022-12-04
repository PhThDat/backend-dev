var jwt: string = localStorage.getItem("jwt")
const toSignIn = () => {
    let signInForm: Promise<Response> = fetch("http://127.0.0.1:4000/file/html?name=sign-in");
    signInForm.then(async (result: Response) => {
        document.write(await result.text());
    });
}

if (jwt === null)
    renderSignIn();
else {
    let jwtSegments: string[] = jwt.split('.');
    let payload = JSON.parse(atob(jwtSegments[1]));
    if (payload.sub === null)
        toSignIn();
    else {
        let userJson: { username: string, email: string, avatarPath: string };

        let req: RequestInit = {
            method: "POST",
            headers: [[ "Authorization", `Bearer ${jwt}` ]],
        }
        let resp: Promise<Response> = fetch("http://127.0.0.1:4000/user/jwt/", req);
        resp.then(async (result: Response) => {
            switch (result.status)
            {
                case 200:
                    userJson = JSON.parse(await result.text());
                    break;
                case 400: case 401:
                    toSignIn();
            }

        });

        document.head.appendChild
    }
}