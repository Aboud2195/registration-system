using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using RegistrationSystem.Api.Data;
using RegistrationSystem.Api.Helpers;
using RegistrationSystem.Api.Helpers.Exceptions;
using RegistrationSystem.Api.Models;
using RegistrationSystem.Api.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RegistrationSystem.Api.Tests
{
    public class AccountControllerTests
    {
        private const string DatabaseName = "Users";
        private const string Path = "/api/Account";
        private const string FakeEmail = "email@email";
        private const string FakePassword = "Password123@!";
        private readonly WebApplicationFactory<Program> factory;
        private readonly IServiceScope scope;
        private readonly ApplicationDbContext context;
        private readonly RestClient restClient;

        public AccountControllerTests()
        {
            this.factory = TestHelpers.UseSqlite(new WebApplicationFactory<Program>(), DatabaseName);
            this.scope = this.factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            this.context = this.CreateCleanContext();
            this.restClient = this.CreateRestClient();
        }

        private ApplicationDbContext CreateCleanContext()
        {
            CleanMemoryDatabase();
            return this.CreateContext();
        }

        private ApplicationDbContext CreateContext()
        {
            var rtn = this.scope.ServiceProvider.GetService<ApplicationDbContext>() ?? throw new NullReferenceException();
            rtn.Database.EnsureCreated();
            Program.SetupIdentitiesAsync(this.scope).Wait();
            return rtn;
        }

        private static SqliteConnection CleanMemoryDatabase()
        {
            var connection = new SqliteConnection(TestHelpers.GetConnectionString(DatabaseName));
            SqliteConnection.ClearPool(connection);
            return connection;
        }

        private RestClient CreateRestClient()
        {
            HttpClient client = this.factory.CreateClient(new WebApplicationFactoryClientOptions());
            RestClient rtn = new(client, new RestClientOptions { FollowRedirects = false, });
            return rtn;
        }

        [Fact]
        public async Task RegisterUser_BodyIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var restRequest = new RestRequest(Path) { Method = Method.Post, };
            restRequest.AddBody($"null");

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_EmptyDto_ShouldReturnBadRequest()
        {
            var restRequest = new RestRequest(Path) { Method = Method.Post, };

            UserRegistrationDto userRegistrationDto = new()
            {
                ConfirmPassword = null!,
                Email = null!,
                FirstName = null!,
                LastName = null!,
                Password = null!,
            };

            restRequest.AddJsonBody(userRegistrationDto);

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_InvalidEmail_ShouldReturnUnprocessableEntityWithProblemDetails()
        {
            var restRequest = new RestRequest(Path) { Method = Method.Post, };

            UserRegistrationDto userRegistrationDto = new()
            {
                Email = "email",
                FirstName = "first",
                LastName = "last",
                Password = "password",
                ConfirmPassword = "confirmPassword",
            };

            restRequest.AddJsonBody(userRegistrationDto);

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

            var problemDetails = DeserializeResponseContent<ValidationProblemDetails>(response);
            problemDetails.Should().BeEquivalentTo(new ValidationProblemDetails
            {
                Detail = "Value should be a valid email.",
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Title = "UserEmail is invalid.",
                Type = "user-email-value-object-invalid",
                BrokenRules = new()
                {
                    new("ValidAsEmail", "Value should be a valid email."),
                },
            });
        }

        [Fact]
        public async Task RegisterUser_EmptyPassword_ShouldReturnUnprocessableEntityWithProblemDetails()
        {
            var restRequest = new RestRequest(Path) { Method = Method.Post, };

            UserRegistrationDto userRegistrationDto = new()
            {
                Email = FakeEmail,
                FirstName = "first",
                LastName = "last",
                Password = string.Empty,
                ConfirmPassword = "confirmPassword",
            };

            restRequest.AddJsonBody(userRegistrationDto);

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

            var problemDetails = DeserializeResponseContent<ValidationProblemDetails>(response);
            problemDetails.Should().BeEquivalentTo(new ValidationProblemDetails
            {
                Detail = "Value is required.",
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Title = "Password is invalid.",
                Type = "password-value-object-invalid",
                BrokenRules = new()
                {
                    new("Required", "Value is required."),
                },
            });
        }

        [Fact]
        public async Task RegisterUser_MismatchPassword_ShouldReturnUnprocessableEntityWithProblemDetails()
        {
            var restRequest = new RestRequest(Path) { Method = Method.Post, };

            UserRegistrationDto userRegistrationDto = new()
            {
                Email = FakeEmail,
                FirstName = "first",
                LastName = "last",
                Password = "password",
                ConfirmPassword = "confirmPassword",
            };

            restRequest.AddJsonBody(userRegistrationDto);

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

            var problemDetails = DeserializeResponseContent<ProblemDetails>(response);
            problemDetails.Should().BeEquivalentTo(new ProblemDetails
            {
                Detail = "The password password and the confirmation password confirmPassword do not match.",
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Title = "The password and confirmation password do not match.",
                Type = "passwords-dont-match",
            });
        }

        [Fact]
        public async Task RegisterUser_ValidDto_ShouldCreateSubscriberUserAndReturnSuccess()
        {
            // Act
            var userDto = await this.CreateUserRequest();

            // Assert
            userDto.Should().BeEquivalentTo(new UserDto
            {
                Email = FakeEmail,
                FirstName = "first",
                LastName = "last",
            }, opt => opt.Excluding(m => m.Id));
            userDto!.Id.Should().NotBeNullOrWhiteSpace();

            var dbUser = this.context.Users.Single(u => u.Email == userDto.Email);
            var dbSubcriberRole = this.context.Roles.Single(r => r.Name == "Subscriber");
            var dbUserRole = this.context.UserRoles.Single(r => r.UserId == dbUser.Id && r.RoleId == dbSubcriberRole.Id);
            dbUser.Id.Should().Be(userDto.Id);
            dbUser.Email.Should().Be(userDto.Email);
            dbUser.UserName.Should().Be(userDto.Email);
            dbUser.FirstName.Should().Be(userDto.FirstName);
            dbUser.LastName.Should().Be(userDto.LastName);
        }

        [Fact]
        public async Task GetUserByIdAsync_NotAuthenticated_ShouldRedirectToLoginPage()
        {
            // Arrange
            var restRequest = new RestRequest($"{Path}/random-id") { Method = Method.Get, };

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Fact]
        public async Task GetUserByIdAsync_IdDoNotExists_ShouldReturnNotFound()
        {
            // Arrange
            await LoginAsAdmin();
            var restRequest = new RestRequest($"{Path}/random-id") { Method = Method.Get, };

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var problemDetails = DeserializeResponseContent<ProblemDetails>(response);
            problemDetails.Should().BeEquivalentTo(new ProblemDetails
            {
                Detail = $"User with id=random-id do not exists.",
                StatusCode = HttpStatusCode.NotFound,
                Title = "User was not found.",
                Type = "user-not-found",
            });
        }

        [Fact]
        public async Task GetUserByIdAsync_IdIsProvided_ShouldReturnTheUserDto()
        {
            // Arrange
            await LoginAsAdmin();

            var createUserDto = (await this.CreateUserRequest())!;
            UserDto? userDto = await GetUser(createUserDto.Id);
            userDto.Should().BeEquivalentTo(createUserDto);
        }

        [Fact]
        public async Task ListUsers_NotAuthenticated_ShouldRedirectToLoginPage()
        {
            // Arrange
            var restRequest = new RestRequest(Path) { Method = Method.Get, };

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Fact]
        public async Task ListUsers_LoggedIn_ShouldReturnTheUserDtoList()
        {
            // Arrange
            await LoginAsAdmin();

            var createUserDto = (await this.CreateUserRequest())!;

            var restRequest = new RestRequest(Path) { Method = Method.Get, };

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dbAdminUser = this.context.Users.Single(u => u.Email == UserService.AdminEmail);

            var userDto = DeserializeResponseContent<List<UserDto>>(response);
            userDto.Should().BeEquivalentTo(new List<UserDto> {
                new UserDto
                {
                    Email = dbAdminUser.Email,
                    FirstName = dbAdminUser.FirstName!,
                    Id = dbAdminUser.Id,
                    LastName = dbAdminUser.LastName!,
                    Confirmed = true,
                },
                createUserDto ,
            });
        }

        [Fact]
        public async Task Login_NotActivated_ShouldRedirectToLoginPage()
        {
            // Arrange
            await this.CreateUserRequest();

            // Act
            var response = await this.LoginAsUser();

            // Assert
            response.IsSuccessful.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Fact]
        public async Task Approve_IdExists_ShouldApproveUser()
        {
            // Arrange
            await LoginAsAdmin();
            var createUserDto = (await this.CreateUserRequest())!;

            // Act
            var response = await this.ApproveUser(createUserDto.Id);
            var userDto = await this.GetUser(createUserDto.Id);

            // Assert
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            userDto!.Confirmed.Should().BeTrue();
        }

        [Fact]
        public async Task Login_Activated_ShouldReturnSuccess()
        {
            // Arrange
            await LoginAsAdmin();
            var createUserDto = (await this.CreateUserRequest())!;
            await ApproveUser(createUserDto.Id);

            // Act
            var response = await this.LoginAsUser();

            // Assert
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RejectUser_IdExists_ShouldDeleteUser()
        {
            // Arrange
            await LoginAsAdmin();
            var createUserDto = (await this.CreateUserRequest())!;

            var restRequest = new RestRequest($"{Path}/{createUserDto.Id}") { Method = Method.Delete, };

            // Act
            var response = await this.restClient.ExecuteAsync(restRequest);

            // Assert
            var userDto = await this.GetUser(createUserDto.Id);
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            userDto.Should().BeNull();
        }

        private async Task LoginAsAdmin()
        {
            var restRequest = new RestRequest($"{Path}/Login") { Method = Method.Post, };
            restRequest.AddJsonBody(new UserLoginDto
            {
                Email = UserService.AdminEmail,
                Password = UserService.AdminPassword,
            });
            var response = await this.restClient.ExecuteAsync(restRequest);
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<UserDto?> CreateUserRequest()
        {
            var restRequest = new RestRequest(Path) { Method = Method.Post, };

            UserRegistrationDto userRegistrationDto = new()
            {
                Email = FakeEmail,
                FirstName = "first",
                LastName = "last",
                Password = FakePassword,
                ConfirmPassword = FakePassword,
            };

            restRequest.AddJsonBody(userRegistrationDto);

            var response = await this.restClient.ExecuteAsync(restRequest);
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            return DeserializeResponseContent<UserDto>(response);
        }

        private async Task<UserDto?> GetUser(string id)
        {
            var restRequest = new RestRequest($"{Path}/{id}") { Method = Method.Get, };
            var response = await this.restClient.ExecuteAsync(restRequest);
            if (!response.IsSuccessful)
            {
                return null;
            }

            var userDto = DeserializeResponseContent<UserDto>(response);
            return userDto;
        }

        private async Task<RestResponse> ApproveUser(string id)
        {
            var restRequest = new RestRequest($"{Path}/{id}/Approve") { Method = Method.Put, };
            return await this.restClient.ExecuteAsync(restRequest);
        }

        private async Task<RestResponse> LoginAsUser()
        {
            var restRequest = new RestRequest($"{Path}/Login") { Method = Method.Post, };
            restRequest.AddJsonBody(new UserLoginDto
            {
                Email = FakeEmail,
                Password = FakePassword,
            });
            return await this.restClient.ExecuteAsync(restRequest);
        }

        private static T? DeserializeResponseContent<T>(RestResponse response)
        {
            T? t = JsonSerializer.Deserialize<T>(response.Content ?? "\"\"", new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return t;
        }
    }
}
