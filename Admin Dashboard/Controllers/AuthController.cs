using Admin_Dashboard.Core.Contents;
using Admin_Dashboard.Core.Dtos.Auth;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Admin_Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authservice;

        public AuthController(IAuthService authservice)
        {
            this.authservice = authservice;
        }

        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedResult = await authservice.SeedRolesAsync();
            return StatusCode(seedResult.StatusCode, seedResult.Message);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            var registerResult = await authservice.RegisterAsync(registerDto);
            return StatusCode(registerResult.StatusCode, registerResult.Message);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginServiceresponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await authservice.LoginAsync(loginDto);

            if (loginResult is null)
                return Unauthorized();

            return Ok(loginResult);
        }

        [HttpPost]
        [Route("update")]
        [Authorize(Roles =StaticUserRole.OWNERADMIN)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var updateroleResulr = await authservice.UpdateRolesAsync(User, updateRoleDto);

            if(updateroleResulr.IsSuccess)
            {
                return Ok(updateroleResulr.Message);
            }
            else
            {
                return StatusCode(updateroleResulr.StatusCode, updateroleResulr.Message);
            }
        }

        [HttpPost]
        [Route("me")]
        public async Task<ActionResult<LoginServiceresponseDto>> Me([FromBody]MeDto token)
        {
            try
            {
                var me = await authservice.MeAsync(token);
                if(me is not null)
                {
                    return Ok(me);
                }
                else
                {
                    return Unauthorized("Invalid Token");
                }
            }
            catch (Exception)
            {
                return Unauthorized("Invalid Token");
            }

        }

        [HttpGet]
        [Route("user")]
        public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUserList()
        {
            var userlist = await authservice.GetUserListAsync();
            return Ok(userlist);
        }

        [HttpGet]
        [Route("user/{userName}")]
        public async Task<ActionResult<UserInfoResult>> GetUserDetaildByUserName([FromBody] string username)
        {
            var user = await authservice.GetUserDetailsByUserNameAsync(username);
            if(user is not null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("UserName not found");
            }
        }

        [HttpGet]
        [Route("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserNameList()
        {
            var usernames = await authservice.GetUserListAsync();
            return Ok(usernames);
        }
    }
}
