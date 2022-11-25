var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var _this = this;
var unameInp = document.getElementById("uname-inp");
var emailInp = document.getElementById("email-inp");
var pwInp = document.getElementById("pw-inp");
var btn = document.getElementById("submit-btn");
var sha256 = function (message) { return __awaiter(_this, void 0, void 0, function () {
    var msgBuffer, hashBuffer, hashArray, hashHex;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                msgBuffer = new TextEncoder().encode(message);
                return [4, crypto.subtle.digest('SHA-256', msgBuffer)];
            case 1:
                hashBuffer = _a.sent();
                hashArray = Array.from(new Uint8Array(hashBuffer));
                hashHex = hashArray.map(function (b) { return b.toString(16).padStart(2, '0'); }).join('');
                return [2, hashHex];
        }
    });
}); };
btn.addEventListener("click", function () { return __awaiter(_this, void 0, void 0, function () {
    var encryptedPw, req, response, _a, jwtObj, _b, _c, _d;
    return __generator(this, function (_e) {
        switch (_e.label) {
            case 0:
                if (!(unameInp.value !== "" && emailInp.value !== "" && pwInp.value !== "")) return [3, 8];
                return [4, sha256(pwInp.value)];
            case 1:
                encryptedPw = _e.sent();
                req = {
                    method: "POST",
                    headers: [["Content-Type", "application/json"]],
                    body: JSON.stringify({ username: unameInp.value, email: emailInp.value, password: encryptedPw })
                };
                return [4, fetch("http://127.0.0.1:4000/user/signup/", req)];
            case 2:
                response = _e.sent();
                _a = response.status;
                switch (_a) {
                    case 201: return [3, 3];
                    case 409: return [3, 5];
                }
                return [3, 7];
            case 3:
                _c = (_b = JSON).parse;
                return [4, response.text()];
            case 4:
                jwtObj = _c.apply(_b, [_e.sent()]);
                localStorage.setItem("jwt", jwtObj.jwt);
                window.open("http://127.0.0.1:4000/user/", "_self");
                return [3, 7];
            case 5:
                _d = alert;
                return [4, response.text()];
            case 6:
                _d.apply(void 0, [_e.sent()]);
                _e.label = 7;
            case 7: return [3, 9];
            case 8:
                alert("Please fill the form");
                _e.label = 9;
            case 9: return [2];
        }
    });
}); });
