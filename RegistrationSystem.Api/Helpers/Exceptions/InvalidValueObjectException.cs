using RegistrationSystem.Api.Helpers.Domain;
using System.Runtime.Serialization;
using System.Text;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class InvalidValueObjectException : HttpException<ValidationProblemDetails>
    {
        protected virtual List<BusinessRule> BrokenRules { get; set; }

        public InvalidValueObjectException(Type valueObjectType, IEnumerable<BusinessRule> brokenRules)
            : base(ProblemTypeTemplate(valueObjectType), ProblemTitleTemplate(valueObjectType), GetDetail(brokenRules), System.Net.HttpStatusCode.UnprocessableEntity)
        {
            this.BrokenRules = brokenRules.ToList();
        }

        protected InvalidValueObjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.BrokenRules = info.Get<List<BusinessRule>>(nameof(this.BrokenRules));
        }

        protected override ValidationProblemDetails GetProblemDetails()
        {
            var rtn = base.GetProblemDetails();
            rtn.BrokenRules = this.BrokenRules.Cast<BusinessRule>().ToList();
            return rtn;
        }

        private static string GetDetail(IEnumerable<BusinessRule> rules)
        {
            StringBuilder issues = new();
            foreach (var businessRule in rules)
            {
                issues.AppendLine(businessRule.Description);
            }

            return issues.ToString().Trim();
        }

        private static string ProblemTypeTemplate(Type valueObjectType) => $"{valueObjectType.Name.Slugify()}-value-object-invalid";

        private static string ProblemTitleTemplate(Type valueObjectType) => $"{valueObjectType.Name} is invalid.";
    }
}
