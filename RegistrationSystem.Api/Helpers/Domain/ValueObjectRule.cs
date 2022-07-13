namespace RegistrationSystem.Api.Helpers.Domain
{
    public enum ValueObjectRuleType
    {
        Required,
        LessThan,
        GreaterThan,
        NotZero,
        InRange,
        NotEmpty,
        NotDistinct,
        UTC,
        NotNullOrEmpty,
        NotNullOrWhiteSpace,
        ShorterThan,
        LongerThan,
        IsNumeric,
        ValidAsUsername,
        ValidAsEmail,
    }

    public class ValueObjectRule : BusinessRule
    {
        public ValueObjectRule(ValueObjectRuleType type, string description)
            : base(type.ToString(), description)
        {
        }
    }
}
