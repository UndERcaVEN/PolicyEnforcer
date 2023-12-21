using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PolicyEnforcer.ServerCore
{
    public class AuthHelper
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

        public static bool ValidateAdmin(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthHelper.GetSymmetricSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token.Replace("Bearer ", string.Empty), validations, out var secureToken);
            var role = claims.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

            return role == "1";
        }
    }
}
