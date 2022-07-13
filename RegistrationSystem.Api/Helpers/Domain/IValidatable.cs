namespace RegistrationSystem.Api.Helpers.Domain
{
    public interface IValidatable
    {
        void ThrowExceptionIfInvalid();
    }
}