using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Models
{
    public class UserId : RequiredValueObject
    {
        public UserId(string value) : base(value)
        {
        }
    }
}
