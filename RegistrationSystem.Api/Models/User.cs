using RegistrationSystem.Api.Data.Models;
using RegistrationSystem.Api.Helpers.Domain;
using RegistrationSystem.Api.Helpers.Exceptions;
using RegistrationSystem.Api.Helpers.Validation;

namespace RegistrationSystem.Api.Models
{
    public class UserRegistrationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

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
                throw new HttpException<ProblemDetails>("passwords-dont-match", "The password and confirmation password do not match.", $"The password {this.password.Value} and the confirmation password {this.confirmPassword.Value} do not match.", System.Net.HttpStatusCode.UnprocessableEntity);
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

    public class User : Entity<UserId>
    {
        //private readonly UserId id;
        //private readonly Name firstName;
        //private readonly Name lastName;
        //private readonly Username userName;
        //private readonly Hash passwordHash;

        private readonly DbUser dbUser;

        public override UserId Id => new UserId(dbUser.Id);

        //private User(UserId id, Name firstName, Name lastName, Username userName, Hash passwordHash)
        //{
        //    this.id = id;
        //    this.firstName = firstName;
        //    this.lastName = lastName;
        //    this.userName = userName;
        //    this.passwordHash = passwordHash;
        //}

        //public User(DbUser dbUser) : this(new UserId(dbUser.Id), new Name(dbUser.FirstName ?? string.Empty), new Name(dbUser.LastName ?? string.Empty), new Username(dbUser.UserName), new Hash(dbUser.PasswordHash))
        //{
        //    this.dbUser = dbUser;
        //}

        public User(DbUser dbUser)
        {
            this.dbUser = dbUser;
            //this.id = new UserId(dbUser.Id);
            //this.firstName = new Name(dbUser.FirstName ?? string.Empty);
            //this.lastName = new Name(dbUser.LastName ?? string.Empty);
            //this.userName = new Username(dbUser.UserName);
            //this.passwordHash = new Hash(dbUser.PasswordHash);
        }

        //public UserDto ToUserDto()
        //{
        //    return new UserDto
        //    {
        //        Id = this.id.Value,
        //        FirstName = this.firstName.Value,
        //        LastName = this.lastName.Value,
        //        UserName = this.userName.Value,
        //    };
        //}
        public UserDto ToUserDto()
        {
            return new UserDto
            {
                Id = this.dbUser.Id,
                FirstName = this.dbUser.FirstName ?? string.Empty,
                LastName = this.dbUser.LastName ?? string.Empty,
                Email = this.dbUser.Email,
            };
        }

        public void SetName(Name firstName, Name lastName)
        {
            this.dbUser.FirstName = firstName.Value;
            this.dbUser.LastName = lastName.Value;
        }

        protected override List<EntityRule> GetBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public class UserId : RequiredValueObject
    {
        public UserId(string value) : base(value)
        {
        }
    }

    public class Name : ValueObject
    {
        private readonly string value;
        public Name(string value)
        {
            this.value = value;
            this.ThrowExceptionIfInvalid();
        }

        public string Value => this.value;

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            var rtn = new List<ValueObjectRule>();
            return rtn;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.value;
        }
    }

    public class Username : RequiredValueObject
    {
        public Username(string value)
            : base(value)
        {
        }

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            var rtn = base.GetBrokenRules();
            rtn.AddRange(this.Value.Is().ValidAsUsername().Done());
            return rtn;
        }
    }

    public class UserEmail : RequiredValueObject
    {
        public UserEmail(string value)
            : base(value)
        {
        }

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            var rtn = base.GetBrokenRules();
            rtn.AddRange(this.Value.Is().ValidAsEmail().Done());
            return rtn;
        }
    }

    public class PasswordHash : RequiredValueObject
    {
        public PasswordHash(string value)
            : base(value)
        {
        }
    }

    public class Password : RequiredValueObject
    {
        public Password(string value)
            : base(value)
        {
        }

        public static bool operator ==(Password id1, Password id2)
        {
            return EqualOperator(id1, id2);
        }

        public static bool operator !=(Password id1, Password id2)
        {
            return NotEqualOperator(id1, id2);
        }
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
