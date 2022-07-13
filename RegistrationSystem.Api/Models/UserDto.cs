namespace RegistrationSystem.Api.Models
{
    /// <summary>
    /// Data transfer object for user data.
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public string Role { get;set; } = string.Empty;
    }
}
