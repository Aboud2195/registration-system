using Microsoft.AspNetCore.Identity;

namespace RegistrationSystem.Api.Data.Models
{
    public class DbUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }


}
