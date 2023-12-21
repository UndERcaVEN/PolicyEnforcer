using PolicyEnforcer.Service.Logging;

namespace PolicyEnforcer.Service.Models
{
    internal class LoginInfo
    {
        internal static LoginInfo Generate() => new()
        {
            Username = string.Format("Client-{0}-{1}", Environment.MachineName, Environment.UserName),
            Password = LoggingHelper.GeneratePassword(10),
        };

        internal string Username { get; set; }
        internal string Password { get; set; }
        public string? Token { get; internal set; }
    }
}
