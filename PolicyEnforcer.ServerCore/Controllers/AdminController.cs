using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using PolicyEnforcer.ServerCore.Database.Context;

namespace PolicyEnforcer.ServerCore.Controllers
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
        
        /// <summary>
        /// Изменяет уровень доступа указанного пользователя
        /// </summary>
        /// <param name="userID">идентификатор целевого пользователя</param>
        /// <param name="newAccessLevel">новый уровень доступа</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("changeaccesslevel")]
        public IActionResult ChangeUserRole(
            [FromForm] Guid userID,
            [FromForm] int newAccessLevel)
        {
            if (!AuthHelper.ValidateAdmin(Request.Headers[HeaderNames.Authorization].ToString()))
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

        /// <summary>
        /// Генерирует запросы на сбор данных об истории посещений сайтов подключенных клиентов
        /// </summary>
        [Authorize]
        [HttpGet("getbrowserhistory")]
        public async Task<IActionResult> RequestBrowserHistory()
        {
            if (!AuthHelper.ValidateAdmin(Request.Headers[HeaderNames.Authorization].ToString()))
            {
                return Unauthorized();
            }

            await _dataHub.Clients.All.SendAsync("GetBrowserHistory", 10);
            return Ok();
        }

        /// <summary>
        /// Генерирует запросы на сбор данных об аппаратных компонентах подключенных клиентов
        /// </summary>
        [HttpGet("gethardwarereadings")]
        [Authorize]
        public async Task<IActionResult> RequestHardwareInfo()
        {
            if (!AuthHelper.ValidateAdmin(Request.Headers[HeaderNames.Authorization].ToString()))
            {
                return Unauthorized();
            }

            await _dataHub.Clients.All.SendAsync("GetHardwareInfo");
            return Ok();
        }

        
    }
}
