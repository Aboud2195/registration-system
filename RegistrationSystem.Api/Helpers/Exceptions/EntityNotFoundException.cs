using RegistrationSystem.Api.Helpers.Domain;
using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class EntityNotFoundException<TEntity> : HttpException<ProblemDetails>
        where TEntity : IEntity
    {
        public EntityNotFoundException(string detail)
            : base(ProblemTypeTemplate, ProblemTitleTemplate, detail, HttpStatusCode.NotFound)
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string ProblemTypeTemplate => $"{typeof(TEntity).Name.Slugify()}-not-found";

        private static string ProblemTitleTemplate => $"{typeof(TEntity).Name} was not found.";
    }
}
