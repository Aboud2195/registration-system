using RegistrationSystem.Api.Data.Models;
using RegistrationSystem.Api.Helpers.Domain;
using RegistrationSystem.Api.Helpers.Exceptions;

namespace RegistrationSystem.Api.Models
{
    /// <summary>
    /// Class resposible of validating a new user's data using value objects and creating the db model.
    /// </summary>
    public class NewUser
    {
        private readonly UserEmail email;
        private readonly Name firstName;
        private readonly Name lastName;
        private readonly Password password;
        private readonly Password confirmPassword;

        public NewUser(UserEmail email, Name firstName, Name lastName, Password password, Password confirmPassword)
        {
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
            this.password = password;
            this.confirmPassword = confirmPassword;
            if (this.confirmPassword != this.password)
            {
                throw new InvalidEntityException("User", new List<BusinessRule> { new("PassowrdsDontMatch", $"The password {this.password.Value} and the confirmation password {this.confirmPassword.Value} do not match.") });
            }
        }

        public NewUser(UserRegistrationDto dto) : this(new UserEmail(dto.Email), new Name(dto.FirstName), new Name(dto.LastName), new Password(dto.Password), new Password(dto.ConfirmPassword))
        {
        }

        public DbUser CreateDbUser()
        {
            return new()
            {
                Email = this.email.Value,
                UserName = this.email.Value,
                FirstName = this.firstName.Value,
                LastName = this.lastName.Value,
            };
        }
    }
}
