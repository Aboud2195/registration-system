namespace RegistrationSystem.Api.Helpers.Domain
{
    public class BusinessRule
    {
        private readonly string description;
        private readonly string type;

        public BusinessRule(string type, string description)
        {
            this.type = type;
            this.description = description;
        }

        public string Type => this.type;

        public string Description => this.description;
    }
}
