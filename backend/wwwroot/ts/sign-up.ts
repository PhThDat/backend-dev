const unameInp: HTMLInputElement = document.getElementById("uname-inp") as HTMLInputElement;
const emailInp: HTMLInputElement = document.getElementById("email-inp") as HTMLInputElement;
const pwInp: HTMLInputElement = document.getElementById("pw-inp") as HTMLInputElement;
const btn: HTMLInputElement = document.getElementById("submit-btn") as HTMLInputElement;

const sha256 = async (message: string) => {
    // encode as UTF-8
    const msgBuffer: Uint8Array = new TextEncoder().encode(message);                    

    // hash the message
    const hashBuffer: ArrayBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);

    // convert ArrayBuffer to Array
    const hashArray: number[] = Array.from(new Uint8Array(hashBuffer));

    // convert bytes to hex string                  
    const hashHex: string = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
    return hashHex;
}

btn.addEventListener("click", async () => {
    if (unameInp.value !== "" && emailInp.value !== "" && pwInp.value !== "") {
        let encryptedPw: string = await sha256(pwInp.value);
        
        let req: RequestInit = {
            method: "POST",
            headers: [[ "Content-Type", "application/json" ]],
            body: JSON.stringify({ username: unameInp.value, email: emailInp.value, password: encryptedPw }),
        }
        let response: Response = await fetch("http://127.0.0.1:4000/user/signup/", req);

        switch (response.status) {
            case 201:
                let jwtObj: { jwt: string } = JSON.parse(await response.text());
                localStorage.setItem("jwt", jwtObj.jwt);
                window.open("http://127.0.0.1:4000/user/", "_self");
                break;
            case 409:
                alert(await response.text());
        }
    }
    else alert("Please fill the form");
});