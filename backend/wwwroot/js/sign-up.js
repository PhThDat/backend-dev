const unameInp = document.getElementById("uname-inp");
const emailInp = document.getElementById("email-inp");
const pwInp = document.getElementById("pw-inp");
const btn = document.getElementById("submit-btn");

btn.addEventListener("click", async () => {
    if (unameInp.value != "" && pwInp.value != "" && emailInp.value != "") {
        var encryptedPw = btoa(pwInp.value);
        var response = await fetch("http://127.0.0.1:4000/user/signup/", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({username: unameInp.value, email: emailInp.value, password: encryptedPw}),
        });
        
        if (response.status == 201)
        {
            var jwtObj = JSON.parse(await response.text());
            localStorage.setItem("jwt", jwtObj.jwt);
            window.open("http://127.0.0.1:4000/user/", "_self");
        }
        else if (response.status == 409)
            alert(await response.text());
    }
    else alert("Please fill the form");
}); 