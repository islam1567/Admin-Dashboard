using Admin_Dashboard.Core.Contents;
using Admin_Dashboard.Core.Dtos.Message;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageservice;

        public MessageController(IMessageService messageservice)
        {
            this.messageservice = messageservice;
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateNewMessage([FromBody] CreateMessageDto createMessageDto)
        {
            var result = await messageservice.CreateNewMessageAsync(User, createMessageDto);
            if (result.IsSuccess)
                return Ok(result.Message);

            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpGet]
        [Route("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMyMessage()
        {
            var message = await messageservice.GetMyMessageAsync(User);
            return Ok(message);
        }

        [HttpGet]   
        [Authorize(Roles =StaticUserRole.OWNERADMIN)]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMessage()
        {
            var message = await messageservice.GetMessageAsync();
            return Ok(message);
        }
    }
}
