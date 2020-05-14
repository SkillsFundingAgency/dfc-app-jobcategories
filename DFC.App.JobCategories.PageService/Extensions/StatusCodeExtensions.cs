using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
