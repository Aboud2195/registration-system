using RegistrationSystem.Api.Helpers.Validation;

namespace RegistrationSystem.Api.Helpers.Domain
{
    public abstract class RequiredValueObject : ValueObject
    {
        private readonly string value;

        protected RequiredValueObject(string value)
        {
            this.value = value;
            this.ThrowExceptionIfInvalid();
        }

        public string Value => this.value;

        protected override List<ValueObjectRule> GetBrokenRules()
        {
            List<ValueObjectRule> rules = new();
            rules.AddRange(this.value.Is().Required().Done());
            return rules;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.value;
        }
    }
}
