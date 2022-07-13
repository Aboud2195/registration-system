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

        protected override List<BusinessRule> GetBrokenRules()
        {
            var rtn = base.GetBrokenRules();
            string regex = @"^[a-zA-Z0-9.! #$%&'*+/=? ^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            if (!Regex.IsMatch(this.Value, regex))
            {
                rtn.Add(new BusinessRule("ValidAsEmail", $"Value should be a valid email."));
            }

            return rtn;
        }
    }
}
