using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PolicyEnforcer.ServerCore.Database.Context;
using PolicyEnforcer.ServerCore.Database.Models;
using PolicyEnforcer.ServerCore.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PolicyEnforcer.ServerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PolicyEnforcerContext _context;
        public UsersController(PolicyEnforcerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Поиск пользователя по идентификатору
        /// </summary>
        /// <param name="userID">идентификатор пользователя</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{userID}")]
        public IActionResult GetUser(Guid userID)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userID);

            if (user is null)
            {
                return BadRequest();
            }

            // Создание конфигурации сопоставления
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserResponseDTO>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
            var result = mapper.Map<UserResponseDTO>(user);

            return Ok(result);
        }

        /// <summary>
        /// Авторизация в системе
        /// </summary>
        /// <param name="loginInfo">Данные для входа</param>
        /// <returns>токен</returns>
        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] UserDTO loginInfo)
        {
            var hashedPassword = AuthHelper.HashString(loginInfo.Password);
            var user = _context.Users.FirstOrDefault(x => x.Login == loginInfo.Login && x.Password == hashedPassword);

            if (user is null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.AccessLevel.ToString()) };
            var jwt = new JwtSecurityToken(
                issuer: AuthHelper.Issuer,
                audience: AuthHelper.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(AuthHelper.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Ok(new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(jwt), UserID = user.UserId });
        }

        /// <summary>
        /// Регистрация в системе
        /// </summary>
        /// <param name="loginInfo">данные для входа</param>
        /// <returns>токен</returns>
        [HttpPost("register")]
        public IActionResult Register(
            [FromBody] UserDTO loginInfo)
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
                Password = AuthHelper.HashString(loginInfo.Password),
                UserId = Guid.NewGuid()
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.AccessLevel.ToString() ) };
            var jwt = new JwtSecurityToken(
                issuer: AuthHelper.Issuer,
                audience: AuthHelper.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(AuthHelper.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Ok(new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(jwt), UserID = user.UserId });
        }
    }
}
