using System;

namespace Project.Performance.Model
{
    public class LoginInfo
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginInfo(string url, string userName, string password)
        {
            this.Url = url;
            this.UserName = userName;
            this.Password = password;
        }
    }
}
