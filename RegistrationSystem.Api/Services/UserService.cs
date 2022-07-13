﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public UserService(UserManager<DbUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
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

            var createdUser = new User(newDbUser);
            return createdUser.ToUserDto();
        }

        private static void ThrowException(IdentityResult result)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new HttpException<ProblemDetails>("identity-error", "An error occured while create the identity", string.Join(Environment.NewLine, errors), System.Net.HttpStatusCode.UnprocessableEntity);
        }

        public async Task<User> GetByIdAsync(UserId id)
        {
            var dbUser = await this.userManager.FindByIdAsync(id.Value);
            if (dbUser == null)
            {
                throw new EntityNotFoundException<User>($"User with id={id.Value} do not exists.");
            }

            return new User(dbUser);
        }

        public async Task<List<User>> ListAsync()
        {
            var dbUsers = await this.userManager.Users.ToListAsync();
            var rtn = dbUsers.Select(u => new User(u)).ToList();
            return rtn;
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
    }
}
