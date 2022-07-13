using RegistrationSystem.Api.Helpers.Domain;
using System.Net;
using System.Runtime.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    public abstract class HttpException : Exception
    {
        public const string ContentType = "application/problem+json";

        private readonly string type;
        private readonly string title;
        private readonly string detail;
        private readonly HttpStatusCode statusCode;

        protected HttpException(string type, string title, string detail, HttpStatusCode statusCode)
            : base(detail)
        {
            this.type = type;
            this.title = title;
            this.detail = detail;
            this.statusCode = statusCode;
        }

        protected HttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.title = info.Get<string>(nameof(this.ProblemTitle));
            this.detail = info.GetOrDefault(nameof(this.ProblemDetail), string.Empty);
            this.type = info.Get<string>(nameof(this.ProblemType));
            this.statusCode = info.Get<HttpStatusCode>(nameof(this.StatusCode));
        }

        public string ProblemType => this.type;

        public string ProblemTitle => this.title;

        public string ProblemDetail => this.detail;

        public HttpStatusCode StatusCode => this.statusCode;

        public abstract Task WriteToHttpContext(HttpContext context);
    }
}