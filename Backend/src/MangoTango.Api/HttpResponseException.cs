using System.Net;

namespace MangoTango.Api
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Description { get; init; }

        public HttpResponseException(HttpStatusCode status, string description) : base()
        {
            this.StatusCode = status;
            this.Description = description;
        }
    }
}
