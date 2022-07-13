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

        /// <summary>
        /// Get user data by its id.
        /// </summary>
        /// <param name="id">The id of the user to retreive.</param>
        /// <returns>The user data if found.</returns>
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

        /// <summary>
        /// List all the users.
        /// Only allowed for the admin.
        /// </summary>
        /// <returns>All the users in the system.</returns>
        [Authorize(Roles = UserService.AdminRole)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> ListAsync()
        {
            return await this.userService.ListAsync();
        }

        /// <summary>
        /// List all the users pending approval.
        /// Only allowed for the admin.
        /// </summary>
        /// <returns>All the pending users in the system.</returns>
        [Authorize(Roles = UserService.AdminRole)]
        [HttpGet("Pending")]
        public async Task<ActionResult<List<UserDto>>> ListPendingAsync()
        {
            return await this.userService.ListPendingAsync();
        }

        /// <summary>
        /// Attempt to login using email and passowrd.
        /// </summary>
        /// <param name="userLoginDto">Required login informations.</param>
        /// <returns>The logged in user info if successful.</returns>
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

        /// <summary>
        /// Attempt to logout the already logged in user.
        /// </summary>
        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Register a new user.
        /// The registered user wont be able to login before he's approved by the admin.
        /// </summary>
        /// <param name="userRegistrationDto">New user information.</param>
        /// <returns>The created user info if successful.</returns>
        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisterUserAsync([FromBody] UserRegistrationDto userRegistrationDto)
        {
            return await this.userService.RegisterUserAsync(userRegistrationDto);
        }

        /// <summary>
        /// Approve a user registration.
        /// Only allowed for the admin.
        /// </summary>
        /// <param name="id">The id of the user to approve.</param>
        [Authorize(Roles = UserService.AdminRole)]
        [HttpPut("{id}/Approve")]
        public async Task<IActionResult> ApproveUser(string id)
        {
            await this.userService.ApproveAsync(new UserId(id));
            return Ok();
        }

        /// <summary>
        /// Reject a user registration.
        /// Only allowed for the admin.
        /// </summary>
        /// <param name="id">The id of the user to reject.</param>
        [Authorize(Roles = UserService.AdminRole)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RejectUser(string id)
        {
            await this.userService.RejectAsync(new UserId(id));
            return Ok();
        }
    }
}
