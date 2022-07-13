using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Helpers.Validation
{
    public class DateTimeBusinessRulesValidator : BusinessRulesValidator<DateTime>
    {
        public DateTimeBusinessRulesValidator(DateTime toValidate, string name)
            : base(toValidate, name)
        {
        }

        public DateTimeBusinessRulesValidator UTC()
        {
            if (this.ToValidate.Kind != DateTimeKind.Utc)
            {
                this.AddBorkenRule(ValueObjectRuleType.UTC, $"{this.Name} should be UTC.");
            }

            return this;
        }
    }
}
