const unameInp = document.getElementById("uname-inp");
const pwInp = document.getElementById("pw-inp");
const btn = document.getElementById("submit-btn");

btn.addEventListener("click", async () => {
    console.log("Clicked");
    if (unameInp.value != "" && pwInp.value != "")
    {     
        var encryptedPw = btoa(pwInp.value);
        var basicContent = btoa(unameInp.value + ':' + encryptedPw);
        var response = await fetch("http://127.0.0.1:4000/user/signin/", {
            method: "POST",
            headers: {
                "Authorization": `BASIC ${basicContent}`,
            },
        });
        if (response.status == 401)
            alert("Incorrect username or password");
        else if (response.status == 200)
        {
            var jwtObj = JSON.parse(await response.text());
            localStorage.setItem("jwt", jwtObj.jwt);
            window.open("http://127.0.0.1:4000/file/html?name=init", "_self");
        }
    }
    else alert("Please fill the form");
});