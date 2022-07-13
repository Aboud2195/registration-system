using RegistrationSystem.Api.Helpers.Domain;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public abstract class InvalidValidatableException<TRule> : HttpException<ValidationProblemDetails>
        where TRule : BusinessRule
    {
        protected InvalidValidatableException(string type, string title, IEnumerable<TRule> brokenRules)
            : base(type, title, GetDetail(brokenRules), HttpStatusCode.UnprocessableEntity)
        {
            this.BrokenRules = brokenRules.ToList();
        }

        protected InvalidValidatableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.BrokenRules = info.Get<List<TRule>>(nameof(this.BrokenRules));
        }

        protected virtual List<TRule> BrokenRules { get; set; }

        private static string GetDetail(IEnumerable<BusinessRule> rules)
        {
            StringBuilder issues = new();
            foreach (var businessRule in rules)
            {
                issues.AppendLine(businessRule.Description);
            }

            return issues.ToString().Trim();
        }

        protected override ValidationProblemDetails GetProblemDetails()
        {
            var rtn = base.GetProblemDetails();
            rtn.BrokenRules = this.BrokenRules.Cast<BusinessRule>().ToList();
            return rtn;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "CloselyRelated")]
    public class ValidationProblemDetails : ProblemDetails
    {
        public List<BusinessRule> BrokenRules { get; set; } = new();
    }
}
