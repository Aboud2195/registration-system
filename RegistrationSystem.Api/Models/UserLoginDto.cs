namespace RegistrationSystem.Api.Models
{
    /// <summary>
    /// Data transfer object for user login request.
    /// </summary>
    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
