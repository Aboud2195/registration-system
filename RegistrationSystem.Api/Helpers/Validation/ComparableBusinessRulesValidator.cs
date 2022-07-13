using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Helpers.Validation
{
    public class ComparableBusinessRulesValidator<T> : BusinessRulesValidator<T>
        where T : IComparable<T>
    {
        public ComparableBusinessRulesValidator(T toValidate, string name)
            : base(toValidate, name)
        {
        }

        public ComparableBusinessRulesValidator<T> LessThan(T max)
        {
            if (this.ToValidate.CompareTo(max) > 0)
            {
                this.AddBorkenRule(ValueObjectRuleType.LessThan, $"{this.Name} with value of {this.ToValidate}, should be less than {max}.");
            }

            return this;
        }

        public ComparableBusinessRulesValidator<T> GreaterThan(T min)
        {
            if (this.ToValidate.CompareTo(min) < 0)
            {
                this.AddBorkenRule(ValueObjectRuleType.GreaterThan, $"{this.Name} with value of {this.ToValidate}, should be greater than {min}.");
            }

            return this;
        }

        public ComparableBusinessRulesValidator<T> NotZero()
        {
            if (this.ToValidate.Equals(0))
            {
                this.AddBorkenRule(ValueObjectRuleType.NotZero, $"{this.Name} should not be zero.");
            }

            return this;
        }

        public ComparableBusinessRulesValidator<T> InRange(T min, T max)
        {
            if (this.ToValidate.CompareTo(max) > 0 || this.ToValidate.CompareTo(min) < 0)
            {
                this.AddBorkenRule(ValueObjectRuleType.InRange, $"{this.Name} with value of {this.ToValidate}, should be between {min} and {max}.");
            }

            return this;
        }
    }
}
