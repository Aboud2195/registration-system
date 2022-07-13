using RegistrationSystem.Api.Helpers.Domain;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class InvalidValueObjectException : InvalidValidatableException<ValueObjectRule>
    {
        public InvalidValueObjectException(Type valueObjectType, IEnumerable<ValueObjectRule> brokenRules)
            : base(ProblemTypeTemplate(valueObjectType), ProblemTitleTemplate(valueObjectType), brokenRules)
        {
        }

        protected InvalidValueObjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string ProblemTypeTemplate(Type valueObjectType) => $"{valueObjectType.Name.Slugify()}-value-object-invalid";

        private static string ProblemTitleTemplate(Type valueObjectType) => $"{valueObjectType.Name} is invalid.";
    }
}
