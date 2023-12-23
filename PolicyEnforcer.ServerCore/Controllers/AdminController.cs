using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PolicyEnforcer.ServerCore.Database.Context;
using PolicyEnforcer.ServerCore.Database.Models;
using PolicyEnforcer.ServerCore.DTO;
using PolicyEnforcer.ServerCore.Hubs;

namespace PolicyEnforcer.ServerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
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
        [HttpPost("changeaccesslevel")]
        public IActionResult ChangeUserRole(
            [FromForm] Guid userID,
            [FromForm] int newAccessLevel)
        {
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
        /// Возвращает историю посещений сайтов для клиента
        /// </summary>
        /// <param name="userID">клиент-цель</param>
        /// <returns></returns>
        [HttpGet("getbrowserhistory/{userID}")]
        public async Task<IActionResult> GetBrowserHistory(Guid userID)
        {
            var target = _context.BrowserHistories.Where(x => x.UserId == userID).OrderByDescending(x => x.DateVisited).Take(150);

            // Создание конфигурации сопоставления
            var config = new MapperConfiguration(cfg => cfg.CreateMap<BrowserHistory, BrowserHistoryDTO>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
            var result = mapper.Map<List<BrowserHistoryDTO>>(target.ToList());

            return Ok(result);
        }
        
        /// <summary>
        /// Возвращает список всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            // Создание конфигурации сопоставления
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserResponseDTO>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
            var users = mapper.Map<List<UserResponseDTO>>(_context.Users.ToList());

            return Ok(users);
        }
        
        [HttpGet("gethardwarereadings/{userID}")]
        public async Task<IActionResult> GetHardwareReadings(Guid userID)
        {
            
            var target = _context.HardwareInfos.Where(x => x.UserId == userID).OrderByDescending(x => x.DateMeasured);

            // Создание конфигурации сопоставления
            var config = new MapperConfiguration(cfg => cfg.CreateMap<HardwareInfo, HardwareInfoDTO>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
            var result = mapper.Map<List<HardwareInfoDTO>>(target.ToList());

            return Ok(result);
        }

        /// <summary>
        /// Генерирует запросы на сбор данных об истории посещений сайтов подключенных клиентов
        /// </summary>
        [HttpGet("requestbrowserhistory")]
        public async Task<IActionResult> RequestBrowserHistory()
        {
            await _dataHub.Clients.All.SendAsync("GetBrowserHistory", 10);
            return Ok();
        }

        /// <summary>
        /// Генерирует запросы на сбор данных об аппаратных компонентах подключенных клиентов
        /// </summary>
        [HttpGet("requesthardwarereadings")]
        public async Task<IActionResult> RequestHardwareInfo()
        {
            await _dataHub.Clients.All.SendAsync("GetHardwareInfo");
            return Ok();
        }
    }
}
