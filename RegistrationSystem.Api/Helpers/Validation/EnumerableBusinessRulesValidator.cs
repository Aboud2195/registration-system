using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Helpers.Validation
{
    public class EnumerableBusinessRulesValidator<T> : BusinessRulesValidator<IEnumerable<T>>
    {
        public EnumerableBusinessRulesValidator(IEnumerable<T> toValidate, string name)
            : base(toValidate, name)
        {
        }

        public EnumerableBusinessRulesValidator<T> NotEmpty()
        {
            if (!this.ToValidate.Any())
            {
                this.AddBorkenRule(ValueObjectRuleType.NotEmpty, $"{this.Name} should not be empty.");
            }

            return this;
        }

        public EnumerableBusinessRulesValidator<T> NotContainsDuplicates()
        {
            var dictinctCount = this.ToValidate.Distinct().Count();
            this.AddBrokenRuleIfNotDistinct(dictinctCount);

            return this;
        }

        public EnumerableBusinessRulesValidator<T> NotContainsDuplicates<TKey>(Func<T, TKey> filter)
        {
            var dictinctCount = this.ToValidate.DistinctBy(filter).Count();
            this.AddBrokenRuleIfNotDistinct(dictinctCount);

            return this;
        }

        private void AddBrokenRuleIfNotDistinct(int dictinctCount)
        {
            if (dictinctCount != this.ToValidate.Count())
            {
                this.AddBorkenRule(ValueObjectRuleType.NotDistinct, $"{this.Name} should not contain duplicates.");
            }
        }
    }
}