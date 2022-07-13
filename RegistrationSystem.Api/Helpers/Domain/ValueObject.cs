using RegistrationSystem.Api.Helpers.Exceptions;

namespace RegistrationSystem.Api.Helpers.Domain
{
    public abstract class ValueObject : Validatable<ValueObjectRule>, IValueObject
    {
        public static bool EqualOperator<TVo>(TVo valueObject1, TVo valueObject2)
            where TVo : ValueObject
        {
            if (valueObject1 is null && valueObject2 is null)
            {
                return true;
            }

            if (valueObject1 is null || valueObject2 is null)
            {
                return false;
            }

            return valueObject1.Equals(valueObject2);
        }

        public static bool NotEqualOperator<TVo>(TVo valueObject1, TVo valueObject2)
            where TVo : ValueObject
        {
            return !EqualOperator(valueObject1, valueObject2);
        }

        public override int GetHashCode()
        {
            return AbstractionsUtilsExtensions.Hash(this.GetEqualityComponents().ToArray());
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        protected override InvalidValidatableException<ValueObjectRule> GetException(IEnumerable<ValueObjectRule> brokenRules)
        {
            return new InvalidValueObjectException(this.GetType(), brokenRules);
        }

        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
