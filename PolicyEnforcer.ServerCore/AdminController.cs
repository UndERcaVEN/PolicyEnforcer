using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using PolicyEnforcer.ServerCore.Database.Context;
using PolicyEnforcer.ServerCore.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PolicyEnforcer.ServerCore
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IHubContext<DataCollectionHub> _dataHub;
        private PolicyEnforcerContext _context;
        public AdminController(IHubContext<DataCollectionHub> dataHub, PolicyEnforcerContext context)
        {
            _dataHub = dataHub;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(
            [FromForm] UserDTO loginInfo)
        {
            var hashedPassword = AuthOptions.HashString(loginInfo.Password);
            var user = _context.Users.FirstOrDefault(x => x.Login == loginInfo.Login && x.Password == hashedPassword);

            if (user is null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.AccessLevel.ToString()) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Ok(new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }

        [Authorize]
        [HttpPost("changeaccesslevel")] 
        public IActionResult ChangeUserRole(
            [FromForm] Guid userID,
            [FromForm] int newAccessLevel)
        {
            var accesstoken = Request.Headers[HeaderNames.Authorization];
            var handler = new JwtSecurityTokenHandler();

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(accesstoken, validations, out var secureToken);
            var role = claims.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

            if (role == "0")
            {
                return Unauthorized();
            }

            var target = _context.Users.FirstOrDefault(x => x.UserId == userID);
            if (target is null || newAccessLevel < 0 || newAccessLevel > 1)
            {
                return BadRequest();
            }

            target.AccessLevel = newAccessLevel;
            _context.Users.Update(target);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("register")]
        public  IActionResult Register(
            UserDTO loginInfo)
        {
            var user = _context.Users.FirstOrDefault(x => x.Login == loginInfo.Login);

            if (user is not null)
            {
                return Conflict();
            }

            user = new User()
            {
                Login = loginInfo.Login,
                AccessLevel = 0,
                Password = AuthOptions.HashString(loginInfo.Password),
                UserId = Guid.NewGuid()
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Ok(new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet("getbrowserhistory")]
        public async void RequestBrowserHistory()
        {
            await _dataHub.Clients.All.SendAsync("GetBrowserHistory", 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("gethardwarereadings")]
        public async void Get()
        {
            await _dataHub.Clients.All.SendAsync("GetHardwareInfo");
        }

    }
}
