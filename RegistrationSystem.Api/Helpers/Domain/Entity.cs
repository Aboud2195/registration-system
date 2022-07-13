using RegistrationSystem.Api.Helpers.Exceptions;

namespace RegistrationSystem.Api.Helpers.Domain
{
    public abstract class Entity<TId> : Validatable<EntityRule>, IEntity
        where TId : ValueObject
    {
        public abstract TId Id { get; }

        protected override InvalidValidatableException<EntityRule> GetException(IEnumerable<EntityRule> brokenRules)
        {
            return new InvalidEntityException(this.GetType(), brokenRules);
        }
    }
}