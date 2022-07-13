using RegistrationSystem.Api.Helpers.Domain;
using System.Text.RegularExpressions;

namespace RegistrationSystem.Api.Models
{
    public class Username : RequiredValueObject
    {
        public Username(string value)
            : base(value)
        {
        }

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            var rtn = base.GetBrokenRules();
            string regex = @"^[a-zA-Z0-9]+([._-]*(?![._-])|[a-zA-Z0-9]*)*[a-zA-Z0-9]*$";
            if (!Regex.IsMatch(this.Value, regex))
            {
                rtn.Add(new ValueObjectRule(ValueObjectRuleType.ValidAsUsername, $"Value should be a valid username."));
            }

            return rtn;
        }
    }
}
