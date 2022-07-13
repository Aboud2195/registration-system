using RegistrationSystem.Api.Helpers.Domain;
using System.Text.RegularExpressions;

namespace RegistrationSystem.Api.Models
{
    public class UserEmail : RequiredValueObject
    {
        public UserEmail(string value)
            : base(value)
        {
        }

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            var rtn = base.GetBrokenRules();
            string regex = @"^[a-zA-Z0-9.! #$%&'*+/=? ^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            if (!Regex.IsMatch(this.Value, regex))
            {
                rtn.Add(new ValueObjectRule(ValueObjectRuleType.ValidAsEmail, $"Value should be a valid email."));
            }

            return rtn;
        }
    }
}
