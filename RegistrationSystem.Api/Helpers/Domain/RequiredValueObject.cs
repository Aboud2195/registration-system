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

        protected override List<BusinessRule> GetBrokenRules()
        {
            List<BusinessRule> rules = new();
            if (this.Value.IsNullOrWhiteSpace())
            {
                rules.Add(new BusinessRule("Required", $"Value is required."));
            }

            return rules;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.value;
        }
    }
}
