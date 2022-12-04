var scrH: number = window.innerHeight, scrW: number = window.innerWidth;
var productsFetch: Promise<Response> = fetch(`http://127.0.0.1:4000/product/goods?keyword=*&amt=${Math.floor(scrW / 200) * 3}`);

const userSection: HTMLAnchorElement = document.getElementById("user-section") as HTMLAnchorElement;
const productList: HTMLElement = document.getElementById("product-list");

productsFetch.then(async (result: Response) => {
    let goodsJson: [{
        goods: {
            id: number,
            name: string,
            description: string | null,
            imagePath: string | null,
        }, 
        amount: number,
    }] = JSON.parse(await result.text());

    goodsJson.forEach((goods) => {
        let goodsPanel: HTMLDivElement = document.createElement("div");
        goodsPanel.classList.add("w-100pct", "h-fit", "flex", "flex-column");
        productList.appendChild(goodsPanel);

        let imgWrapper: HTMLDivElement = document.createElement("div");
        imgWrapper.classList.add("overflow-hidden", "sqr-fit");
        goodsPanel.appendChild(imgWrapper);

        let goodsImg: HTMLImageElement = document.createElement("img");
        goodsImg.classList.add("w-100pct");
        if (goods.goods.imagePath != null)
            goodsImg.src = `http://127.0.0.1:4000/file/img?name=${goods.goods.imagePath}`;
        imgWrapper.appendChild(goodsImg);

        let goodsName: HTMLHeadingElement = document.createElement("h2");
        goodsName.innerHTML = goods.goods.name;
        goodsName.classList.add("align-self-center");
        goodsPanel.appendChild(goodsName);

        let goodsDesc: HTMLParagraphElement = document.createElement("p");
        goodsDesc.innerHTML = goods.goods.description;
        goodsPanel.appendChild(goodsDesc);
    });
});

const renderSignIn = () => {
    userSection.innerHTML = "Sign In";
    userSection.href = "http://127.0.0.1:4000/file/html?name=sign-in";
}


var jwt: string = localStorage.getItem("jwt");
if (jwt == null)
    renderSignIn();
else {
    var req: RequestInit = {
        method: "POST",
        headers: [[ "Authorization", `Bearer ${jwt}`]],
    };
    var jwtAuth: Promise<Response> = fetch("http://127.0.0.1:4000/user/jwt/", req);
    jwtAuth.then((result: Response) => {
        if (result.status == 200)
        {
            let jwtPayload = JSON.parse(atob(jwt.split('.')[1]));
            userSection.innerHTML = jwtPayload.sub;
            userSection.href = "http://127.0.0.1:4000/user/";
        }
        else renderSignIn();
    });
}