namespace RegistrationSystem.Api.Helpers.Domain
{
    public enum EntityRuleType
    {
        SubscriptionNotFoundForOrganization,
        SubscriptionOrganizationMismatch,
    }

    public class EntityRule : BusinessRule
    {
        public EntityRule(EntityRuleType type, string description)
            : base(type.ToString(), description)
        {
        }
    }
}
