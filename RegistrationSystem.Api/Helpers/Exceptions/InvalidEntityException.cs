using RegistrationSystem.Api.Helpers.Domain;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class InvalidEntityException : InvalidValidatableException<EntityRule>
    {
        public InvalidEntityException(Type entityType, IEnumerable<EntityRule> brokenRules)
            : base(ProblemTypeTemplate(entityType), ProblemTitleTemplate(entityType), brokenRules)
        {
        }

        protected InvalidEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string ProblemTypeTemplate(Type entityType) => $"{entityType.Name.Slugify()}-entity-invalid";

        private static string ProblemTitleTemplate(Type entityType) => $"{entityType.Name} is invalid.";
    }
}
