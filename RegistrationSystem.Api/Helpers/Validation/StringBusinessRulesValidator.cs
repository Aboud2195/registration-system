using RegistrationSystem.Api.Helpers.Domain;
using System.Text.RegularExpressions;

namespace RegistrationSystem.Api.Helpers.Validation
{
    public class StringBusinessRulesValidator : BusinessRulesValidator<string>
    {
        public StringBusinessRulesValidator(string toValidate, string name)
            : base(toValidate ?? string.Empty, name)
        {
        }

        public StringBusinessRulesValidator NotNullOrEmpty()
        {
            if (this.ToValidate.IsNullOrEmpty())
            {
                this.AddBorkenRule(ValueObjectRuleType.NotNullOrEmpty, $"{this.Name} should not be null or empty.");
            }

            return this;
        }

        public StringBusinessRulesValidator NotNullOrWhiteSpace()
        {
            if (this.ToValidate.IsNullOrWhiteSpace())
            {
                this.AddBorkenRule(ValueObjectRuleType.NotNullOrWhiteSpace, $"{this.Name} should not be null or empty.");
            }

            return this;
        }

        public StringBusinessRulesValidator Required()
        {
            if (this.ToValidate.IsNullOrWhiteSpace())
            {
                this.AddBorkenRule(ValueObjectRuleType.Required, $"{this.Name} is required.");
                this.SetIsFatal();
            }

            return this;
        }

        public StringBusinessRulesValidator ShorterThan(int maxLength)
        {
            if (this.ToValidate.Length >= maxLength)
            {
                this.AddBorkenRule(ValueObjectRuleType.ShorterThan, $"{this.Name} should be shorter than {maxLength}.");
            }

            return this;
        }

        public StringBusinessRulesValidator LongerThan(int minLength)
        {
            if (this.ToValidate.Length <= minLength)
            {
                this.AddBorkenRule(ValueObjectRuleType.LongerThan, $"{this.Name} should be longer than {minLength}.");
            }

            return this;
        }

        public StringBusinessRulesValidator IsNumeric()
        {
            string regex = @"\d*";
            if (!Regex.IsMatch(this.ToValidate, regex))
            {
                this.AddBorkenRule(ValueObjectRuleType.IsNumeric, $"{this.Name} should be numeric.");
            }

            return this;
        }

        public StringBusinessRulesValidator ValidAsUsername()
        {
            string regex = @"^[a-zA-Z0-9]+([._-]*(?![._-])|[a-zA-Z0-9]*)*[a-zA-Z0-9]*$";
            if (!Regex.IsMatch(this.ToValidate, regex))
            {
                this.AddBorkenRule(ValueObjectRuleType.ValidAsUsername, $"{this.Name} should be a valid username.");
            }

            return this;
        }

        public StringBusinessRulesValidator ValidAsEmail()
        {
            string regex = @"^[a-zA-Z0-9.! #$%&'*+/=? ^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            if (!Regex.IsMatch(this.ToValidate, regex))
            {
                this.AddBorkenRule(ValueObjectRuleType.ValidAsEmail, $"{this.Name} should be a valid email.");
            }

            return this;
        }
    }
}