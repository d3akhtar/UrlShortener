using System.Diagnostics.Eventing.Reader;
using System.Net;

namespace UrlShortener.Model
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public HttpStatusCode StatusCode{ get; set; }
        public string RedirectUrl { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
