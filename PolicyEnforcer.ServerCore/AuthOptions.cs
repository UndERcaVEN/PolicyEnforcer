using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace PolicyEnforcer.ServerCore
{
    public class AuthOptions
    {
        public const string Issuer = "PolicyEnforcerServer";
        public const string Audience = "PolicyEnforcerClient";
        const string key = "chuchikmuchik731";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        public static byte[] HashString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return MD5.HashData(bytes);
        }
    }
}
