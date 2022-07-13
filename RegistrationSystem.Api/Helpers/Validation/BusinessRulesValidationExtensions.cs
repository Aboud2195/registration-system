namespace RegistrationSystem.Api.Helpers.Validation
{
    public static class BusinessRulesValidationExtensions
    {
        public static ComparableBusinessRulesValidator<T> Is<T>(this T value, [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string callerExp = "")
            where T : IComparable<T>
        {
            return new ComparableBusinessRulesValidator<T>(value, callerExp);
        }

        public static StringBusinessRulesValidator Is(this string value, [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string callerExp = "")
        {
            return new StringBusinessRulesValidator(value, callerExp);
        }

        public static DateTimeBusinessRulesValidator Is(this DateTime value, [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string callerExp = "")
        {
            return new DateTimeBusinessRulesValidator(value, callerExp);
        }

        public static EnumerableBusinessRulesValidator<T> Is<T>(this IEnumerable<T> value, [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string callerExp = "")
        {
            return new EnumerableBusinessRulesValidator<T>(value, callerExp);
        }
    }
}
