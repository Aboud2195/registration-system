using RegistrationSystem.Api.Helpers.Exceptions;

namespace RegistrationSystem.Api.Helpers.Domain
{
    public abstract class Validatable<TRule> : IValidatable
        where TRule : BusinessRule
    {
        protected Validatable()
        {
        }

        protected abstract List<TRule> GetBrokenRules();

        public void ThrowExceptionIfInvalid()
        {
            var brokenRules = this.GetBrokenRules();
            if (brokenRules.Count > 0)
            {
                throw this.GetException(brokenRules);
            }
        }

        protected abstract InvalidValidatableException<TRule> GetException(IEnumerable<TRule> brokenRules);
    }
}
