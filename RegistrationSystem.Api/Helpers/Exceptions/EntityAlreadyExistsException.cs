using RegistrationSystem.Api.Helpers.Domain;
using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    [Serializable]
    public class EntityAlreadyExistsException<TEntity> : HttpException<ProblemDetails>
        where TEntity : IEntity
    {
        public EntityAlreadyExistsException(string detail)
            : base(ProblemTypeTemplate, ProblemTitleTemplate, detail, HttpStatusCode.Conflict)
        {
        }

        protected EntityAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string ProblemTypeTemplate => $"{typeof(TEntity).Name.Slugify()}-already-exists";

        private static string ProblemTitleTemplate => $"{typeof(TEntity).Name} already exists.";
    }
}
