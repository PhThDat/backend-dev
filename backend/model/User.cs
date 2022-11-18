namespace BackEndCSharp.Model;

class UserAccount
{
    public string Username;
    public string Email;
    public string Password;
    public UserAccount(string username, string email, string pwd)
    {
        Username = username;
        Email = email;
        Password = pwd;
    }
}