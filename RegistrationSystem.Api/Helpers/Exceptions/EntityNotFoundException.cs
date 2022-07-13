using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : HttpException<ProblemDetails>
    {
        public EntityNotFoundException(string name ,string detail)
            : base(GetProblemTypeTemplate(name), GetProblemTitleTemplate(name), detail, HttpStatusCode.NotFound)
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string GetProblemTypeTemplate(string name) => $"{name.Slugify()}-not-found";

        private static string GetProblemTitleTemplate(string name) => $"{name} was not found.";
    }
}
