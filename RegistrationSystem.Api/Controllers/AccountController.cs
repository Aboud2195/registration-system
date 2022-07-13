using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistrationSystem.Api.Data.Models;
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

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var signInResult = await this.signInManager.PasswordSignInAsync(userLoginDto.Email, userLoginDto.Password, false, false);
            if (signInResult.Succeeded)
            {
                return Ok();
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost("Logout")]
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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(string id)
        {
            var user = await this.userService.GetByIdAsync(new UserId(id));
            return user.ToUserDto();
        }

        [Authorize(Roles = UserService.AdminRole)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> ListAsync()
        {
            var users = await this.userService.ListAsync();
            return users.Select(u => u.ToUserDto()).ToList();
        }
    }
}
