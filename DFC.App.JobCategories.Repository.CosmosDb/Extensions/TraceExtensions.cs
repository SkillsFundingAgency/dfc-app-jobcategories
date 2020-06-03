using DFC.App.JobCategories.Data.Models;
using System;
using System.Diagnostics;

namespace DFC.App.JobCategories.Repository.CosmosDb.Extensions
{
    public static class TraceExtensions
    {
        public static void AddTraceInformation(this RequestTrace trace)
        {
            if (trace == null)
            {
                throw new ArgumentException($"{nameof(trace)} cannot be null");
            }

            trace.AddTraceId(Activity.Current.TraceId.ToString());
            trace.AddParentId(Activity.Current.ParentId.ToString());
        }
    }
}
