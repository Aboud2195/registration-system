using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Models
{
    public class Name : ValueObject
    {
        private readonly string value;
        public Name(string value)
        {
            this.value = value;
            this.ThrowExceptionIfInvalid();
        }

        public string Value => this.value;

        protected override List<BusinessRule> GetBrokenRules()
        {
            var rtn = new List<BusinessRule>();
            return rtn;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.value;
        }
    }
}
