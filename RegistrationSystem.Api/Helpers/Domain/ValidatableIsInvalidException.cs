using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Domain
{
    [Serializable]
    public class ValidatableIsInvalidException : Exception
    {
        public ValidatableIsInvalidException(string message)
            : base(message)
        {
        }

        protected ValidatableIsInvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
