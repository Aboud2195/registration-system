using System.Net;
using System.Text.Json.Serialization;

namespace RegistrationSystem.Api.Helpers.Exceptions
{
    public class ProblemDetails
    {
        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public int Status { get => (int)this.StatusCode; set => this.StatusCode = (HttpStatusCode)value; }

        public string Detail { get; set; } = string.Empty;

        internal string Instance { get; set; } = string.Empty;
    }
}