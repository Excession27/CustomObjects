namespace CustomObjects.Models
{
    public class LogInInfo
    {
        public LogInInfo(string username, string password, string token) 
        {
            this.Username = username;
            this.Password = password;
            this.SecurityToken = token;
        }
        public string Username { get; }
        public string Password { get; }
        public string SecurityToken { get; }
    }
}
