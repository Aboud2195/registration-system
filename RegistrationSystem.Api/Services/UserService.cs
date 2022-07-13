using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationSystem.Api.Data;
using RegistrationSystem.Api.Data.Models;
using RegistrationSystem.Api.Helpers.Exceptions;
using RegistrationSystem.Api.Models;

namespace RegistrationSystem.Api.Services
{
    public class UserService
    {
        internal const string SubscriberRole = "Subscriber";
        internal const string AdminRole = "Admin";
        internal const string AdminEmail = "admin@site.com";
        internal const string AdminPassword = "123456qQ!";

        private readonly UserManager<DbUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;

        public UserService(UserManager<DbUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        /// <param name="userRegistrationDto">New user info.</param>
        /// <returns>The regitered user data.</returns>
        public async Task<UserDto> RegisterUserAsync(UserRegistrationDto userRegistrationDto)
        {
            var newUser = new NewUser(userRegistrationDto);
            var newDbUser = newUser.CreateDbUser();
            var result = await this.userManager.CreateAsync(newDbUser, userRegistrationDto.Password);
            if (!result.Succeeded)
            {
                ThrowException(result);
            }

            result = await this.userManager.AddToRoleAsync(newDbUser, SubscriberRole);
            if (!result.Succeeded)
            {
                ThrowException(result);
            }

            return await this.ToUserDtoAsync(newDbUser);
        }

        private static void ThrowException(IdentityResult result)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new HttpException<ProblemDetails>("identity-error", "An error occured while create the identity", string.Join(Environment.NewLine, errors), System.Net.HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Get user data by email.
        /// </summary>
        /// <param name="email">The email of the user to find.</param>
        /// <exception cref="HttpException{ProblemDetails}">This exception is thrown if the user was not found.</exception>
        /// <returns>The user data if found.</returns>
        public async Task<UserDto> GetUserByEmailAsync(UserEmail email)
        {
            var dbUser = await this.userManager.FindByEmailAsync(email.Value);
            if (dbUser == null)
            {
                throw new EntityNotFoundException("User", $"User with email={email.Value} do not exists.");
            }

            return await this.ToUserDtoAsync(dbUser);
        }

        /// <summary>
        /// Get user data by id.
        /// </summary>
        /// <param name="id">The id of the user to find.</param>
        /// <exception cref="HttpException{ProblemDetails}">This exception is thrown if the user was not found.</exception>
        /// <returns>The user data if found.</returns>
        public async Task<DbUser> GetByIdAsyncAsync(UserId id)
        {
            var dbUser = await this.userManager.FindByIdAsync(id.Value);
            if (dbUser == null)
            {
                throw new EntityNotFoundException("User", $"User with id={id.Value} do not exists.");
            }

            return dbUser;
        }

        /// <summary>
        /// Get all the users.
        /// </summary>
        /// <returns>List of all the users.</returns>
        public async Task<List<UserDto>> ListAsync()
        {
            var dbUsers = await this.userManager.Users.ToListAsync();
            var rtn = new List<UserDto>();
            foreach (var dbUser in dbUsers)
            {
                rtn.Add(await this.ToUserDtoAsync(dbUser));
            }

            return rtn;
        }

        /// <summary>
        /// Get all the users pending approval.
        /// </summary>
        /// <returns>List of all the pending users.</returns>
        public async Task<List<UserDto>> ListPendingAsync()
        {
            var dbUsers = await this.userManager.Users.Where(u => !u.EmailConfirmed).ToListAsync();
            var rtn = new List<UserDto>();
            foreach (var dbUser in dbUsers)
            {
                rtn.Add(await this.ToUserDtoAsync(dbUser));
            }

            return rtn;
        }

        /// <summary>
        /// Approve a user.
        /// </summary>
        /// <param name="id">The id of the user to approve.</param>
        public async Task ApproveAsync(UserId id)
        {
            var dbUser = await this.context.Users.SingleAsync(u => u.Id == id.Value);
            dbUser.EmailConfirmed = true;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Reject and delete a user.
        /// </summary>
        /// <param name="id">The id of the user to reject.</param>
        public async Task RejectAsync(UserId id)
        {
            var dbUser = await this.userManager.FindByIdAsync(id.Value);
            await this.userManager.DeleteAsync(dbUser);
        }

        /// <summary>
        /// Convert db model to data transfer object.
        /// </summary>
        /// <param name="id">The id of the user to approve.</param>
        public async Task<UserDto> ToUserDtoAsync(DbUser dbUser)
        {
            var dbUserRole = await this.context.UserRoles.SingleAsync(r => r.UserId == dbUser.Id);
            var dbSubcriberRole = await this.context.Roles.SingleAsync(r => r.Id == dbUserRole.RoleId);
            return new UserDto
            {
                Id = dbUser.Id,
                FirstName = dbUser.FirstName ?? string.Empty,
                LastName = dbUser.LastName ?? string.Empty,
                Email = dbUser.Email,
                Confirmed = dbUser.EmailConfirmed,
                Role = dbSubcriberRole.Name,
            };
        }

        internal async Task RegisterRolesIfDoesntExistsAsync()
        {
            if (await this.roleManager.Roles.AnyAsync())
            {
                return;
            }

            await this.roleManager.CreateAsync(new IdentityRole(AdminRole));
            await this.roleManager.CreateAsync(new IdentityRole(SubscriberRole));
        }

        internal async Task RegisterAdminIfDoesntExistsAsync()
        {
            var admins = await this.userManager.GetUsersInRoleAsync(AdminRole);
            if (admins.Any())
            {
                return;
            }

            var newUser = new NewUser(new()
            {
                Email = AdminEmail,
                FirstName = AdminRole,
                Password = AdminPassword,
                ConfirmPassword = AdminPassword,
                LastName = string.Empty,
            });

            var newDbUser = newUser.CreateDbUser();
            newDbUser.EmailConfirmed = true;

            var result = await this.userManager.CreateAsync(newDbUser, AdminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create admin.");
            }

            await this.userManager.AddToRoleAsync(newDbUser, AdminRole);
        }
    }
}
