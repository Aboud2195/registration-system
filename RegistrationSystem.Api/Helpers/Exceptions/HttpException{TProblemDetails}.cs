using RegistrationSystem.Api.Helpers.Domain;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    public class HttpException<TProblemDetails> : HttpException
        where TProblemDetails : ProblemDetails, new()
    {
        public HttpException(string type, string title, string detail, HttpStatusCode statusCode)
            : base(type, title, detail, statusCode)
        {
        }

        public HttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override Task WriteToHttpContext(HttpContext context)
        {
            TProblemDetails problemDetails = this.GetProblemDetails();
            context.Response.StatusCode = problemDetails.Status;
            problemDetails.Instance = context.Request.Path;
            return context.Response.WriteAsJsonAsync(problemDetails, null, HttpException.ContentType);
        }

        protected virtual TProblemDetails GetProblemDetails()
        {
            return new TProblemDetails
            {
                StatusCode = this.StatusCode,
                Detail = this.ProblemDetail,
                Type = this.ProblemType,
                Title = this.ProblemTitle,
            };
        }
    }
}