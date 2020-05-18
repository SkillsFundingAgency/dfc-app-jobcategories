using System.Net;

namespace DFC.App.JobCategories.PageService.Extensions
{
    public static class StatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }
    }
}
