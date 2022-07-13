using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistrationSystem.Api.Data.Models;
using RegistrationSystem.Api.Helpers;
using RegistrationSystem.Api.Models;
using RegistrationSystem.Api.Services;

namespace RegistrationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<DbUser> signInManager;
        private readonly UserService userService;

        public AccountController(SignInManager<DbUser> signInManager, UserService userService)
        {
            this.signInManager = signInManager;
            this.userService = userService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(string id)
        {
            var dbUser = await this.userService.GetByIdAsyncAsync(new UserId(id));
            if(dbUser == null || dbUser.Id.IsNullOrWhiteSpace())
            {
                return NotFound();
            }

            return await this.userService.ToUserDtoAsync(dbUser);
        }

        [Authorize(Roles = UserService.AdminRole)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> ListAsync()
        {
            return await this.userService.ListAsync();
        }

        [Authorize(Roles = UserService.AdminRole)]
        [HttpGet("Pending")]
        public async Task<ActionResult<List<UserDto>>> ListPendingAsync()
        {
            return await this.userService.ListPendingAsync();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var signInResult = await this.signInManager.PasswordSignInAsync(userLoginDto.Email, userLoginDto.Password, false, false);
            if (signInResult.Succeeded)
            {
                return await this.userService.GetUserByEmailAsync(new UserEmail(userLoginDto.Email));
            }

            return Forbid();
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult<UserDto>> Logout()
        {
            await this.signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisterUserAsync([FromBody] UserRegistrationDto userRegistrationDto)
        {
            return await this.userService.RegisterUserAsync(userRegistrationDto);
        }

        [Authorize(Roles = UserService.AdminRole)]
        [HttpPut("{id}/Approve")]
        public async Task<IActionResult> ApproveUser(string id)
        {
            await this.userService.ApproveAsync(new UserId(id));
            return Ok();
        }

        [Authorize(Roles = UserService.AdminRole)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RejectUser(string id)
        {
            await this.userService.RejectAsync(new UserId(id));
            return Ok();
        }
    }
}
