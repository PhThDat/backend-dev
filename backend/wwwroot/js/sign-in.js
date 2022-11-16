const unameInp = document.getElementById("uname-inp");
const pwInp = document.getElementById("pw-inp");
const btn = document.getElementById("submit-btn");

btn.addEventListener("click", async () => {
    if (unameInp.value != "" && pwInp.value != "")
    {
        var encryptedPw = btoa(pwInp.value);
        var basicContent = btoa(unameInp.value + ':' + encryptedPw);
        var req = await fetch("http://127.0.0.1:4000/user/signin/", {
            method: "POST",
            headers: {
                "Authorization": "BASIC " + basicContent,
            },
        });
        
        var body = await req.text();
        if (req.status == 401)
            alert(body);
        else window.open(body, "_self");
    }
})