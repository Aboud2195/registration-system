using RegistrationSystem.Api.Helpers.Domain;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class InvalidEntityException : InvalidValidatableException
    {
        public InvalidEntityException(string name, IEnumerable<BusinessRule> brokenRules)
            : base(GetProblemTypeTemplate(name), GetProblemTitleTemplate(name), brokenRules)
        {
        }

        protected InvalidEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string GetProblemTypeTemplate(string name) => $"{name.Slugify()}-entity-invalid";

        private static string GetProblemTitleTemplate(string name) => $"{name} is invalid.";
    }
}
