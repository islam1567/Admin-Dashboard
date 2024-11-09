using Admin_Dashboard.Core.Dtos.Log;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService logService;

        public LogController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpGet]
        [Authorize(Roles ="OWNER,ADMIN")]
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetLog()
        {
            var logs = await logService.GetLogAsync();
            return Ok(logs);
        }

        [HttpGet]
        [Route("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLog()
        {
            var logs = await logService.GetMyLogAsync(User);
            return Ok(logs);
        }
    }
}
