using DFC.App.JobCategories.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Extensions
{
    public static class EventGridTracingExtensions
    {
        public static IApplicationBuilder UseEventGridTracing(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EventGridTracingMiddleware>();
        }
    }
}
