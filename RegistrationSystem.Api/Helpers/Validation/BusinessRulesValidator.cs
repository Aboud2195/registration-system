using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Helpers.Validation
{
    public abstract class BusinessRulesValidator<T>
    {
        private readonly T toValidate;
        private readonly string name;
        private readonly List<ValueObjectRule> brokenBusinessRules = new();
        private bool isFatal;

        protected BusinessRulesValidator(T toValidate, string name)
        {
            this.toValidate = toValidate;
            this.name = name.Replace("this.", string.Empty);
        }

        protected T ToValidate => this.toValidate;

        protected string Name => this.name;

        protected List<ValueObjectRule> BrokenBusinessRules => this.brokenBusinessRules;

        protected bool IsFatal => this.isFatal;

        protected void AddBorkenRule(ValueObjectRuleType type, string description)
        {
            if (this.isFatal)
            {
                return;
            }

            this.BrokenBusinessRules.Add(new ValueObjectRule(type, description));
        }

        protected void SetIsFatal()
        {
            this.isFatal = true;
        }

        public List<ValueObjectRule> Done()
        {
            return this.BrokenBusinessRules;
        }
    }
}
