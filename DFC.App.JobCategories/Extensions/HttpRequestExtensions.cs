using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DFC.App.JobCategories.Extensions
{
    public static class HttpRequestExtensions
    {
        private static List<string> validSchemes = new List<string> { "http://", "https://" };

        public static Uri? GetBaseAddress(this HttpRequest request, IUrlHelper? urlHelper = null)
        {
            if (request != null)
            {
                if (request.Headers.TryGetValue("x-forwarded-proto", out var forwardedProtocol)
                    && request.Headers.TryGetValue("x-original-host", out var originalHost))
                {
                    return new Uri($"{forwardedProtocol}://{originalHost}");
                }

                var destinationUri = string.IsNullOrWhiteSpace(request.Scheme) ? default : new Uri($"{request.Scheme}://{request.Host}{urlHelper?.Content("~")}");

                if (destinationUri != null && validSchemes.Contains(destinationUri.Scheme))
                {
                    return destinationUri;
                }
            }

            return default;
        }
    }
}