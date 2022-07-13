using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class InvalidActionException<TProblemDetails> : HttpException<TProblemDetails>
        where TProblemDetails : ProblemDetails, new()
    {
        public InvalidActionException(string type, string title, string detail)
            : base(type, title, detail, HttpStatusCode.Forbidden)
        {
        }

        protected InvalidActionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
