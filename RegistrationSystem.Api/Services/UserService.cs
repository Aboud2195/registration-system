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

            return this.ToUserDto(newDbUser);
        }

        private static void ThrowException(IdentityResult result)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new HttpException<ProblemDetails>("identity-error", "An error occured while create the identity", string.Join(Environment.NewLine, errors), System.Net.HttpStatusCode.UnprocessableEntity);
        }

        public async Task<UserDto> GetUserByEmailAsync(UserEmail email)
        {
            var dbUser = await this.userManager.FindByEmailAsync(email.Value);
            if (dbUser == null)
            {
                throw new EntityNotFoundException("User", $"User with email={email.Value} do not exists.");
            }

            return this.ToUserDto(dbUser);
        }

        public async Task<DbUser> GetByIdAsyncAsync(UserId id)
        {
            var dbUser = await this.userManager.FindByIdAsync(id.Value);
            if (dbUser == null)
            {
                throw new EntityNotFoundException("User", $"User with id={id.Value} do not exists.");
            }

            return dbUser;
        }

        public async Task<List<UserDto>> ListAsync()
        {
            var dbUsers = await this.userManager.Users.ToListAsync();
            var rtn = dbUsers.Select(u => this.ToUserDto(u)).ToList();
            return rtn;
        }

        public async Task ApproveAsync(UserId id)
        {
            var dbUser = await this.context.Users.SingleAsync(u => u.Id == id.Value);
            dbUser.EmailConfirmed = true;
            await this.context.SaveChangesAsync();
        }

        public async Task RejectAsync(UserId id)
        {
            var dbUser = await this.userManager.FindByIdAsync(id.Value);
            await this.userManager.DeleteAsync(dbUser);
        }

        public async Task RegisterRolesIfDoesntExistsAsync()
        {
            if (await this.roleManager.Roles.AnyAsync())
            {
                return;
            }

            await this.roleManager.CreateAsync(new IdentityRole(AdminRole));
            await this.roleManager.CreateAsync(new IdentityRole(SubscriberRole));
        }

        public async Task RegisterAdminIfDoesntExistsAsync()
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

        public UserDto ToUserDto(DbUser dbUser)
        {
            return new UserDto
            {
                Id = dbUser.Id,
                FirstName = dbUser.FirstName ?? string.Empty,
                LastName = dbUser.LastName ?? string.Empty,
                Email = dbUser.Email,
                Confirmed = dbUser.EmailConfirmed,
            };
        }
    }
}
