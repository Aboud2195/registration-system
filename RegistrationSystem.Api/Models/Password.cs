using RegistrationSystem.Api.Helpers.Domain;

namespace RegistrationSystem.Api.Models
{
    public class Password : RequiredValueObject
    {
        public Password(string value)
            : base(value)
        {
        }

        public static bool operator ==(Password id1, Password id2)
        {
            return EqualOperator(id1, id2);
        }

        public static bool operator !=(Password id1, Password id2)
        {
            return NotEqualOperator(id1, id2);
        }
    }
}
