const unameInp = document.getElementById("uname-inp");
const emailInp = document.getElementById("email-inp");
const pwInp = document.getElementById("pw-inp");
const btn = document.getElementById("submit-btn");

btn.addEventListener("click", async () => {
    if (unameInp.value != "" && pwInp.value != "" && emailInp.value != "") {
        var encryptedPw = btoa(pwInp.value);
        var req = await fetch("http://127.0.0.1:4000/user/signup/",
            {
                headers: {
                    'Content-Type': 'application/json',
                },
                method: "POST",
                body: JSON.stringify({username: unameInp.value, email: emailInp.value, password: encryptedPw}),
            });
        
        var reader = new FileReader();
        reader.readAsDataURL(await req.blob()); 
        reader.onloadend = () => {
            var base64data = reader.result;
            localStorage.setItem("jwt", base64data.slice(base64data.length - 44));
        }
    }
    else alert("Please fill the form");
});