using System;

namespace DFC.App.JobCategories.PageService.Extensions
{
    public static class UriExtensions
    {
        public static string GetContentItemType(this Uri value)
        {
            if (value == null)
            {
                throw new InvalidOperationException($"{nameof(value)} is null");
            }

            return value.Segments[1];
        }

        public static Guid GetContentItemId(this Uri value)
        {
            if (value == null)
            {
                throw new InvalidOperationException($"{nameof(value)} is null");
            }

            return Guid.Parse(value.Segments[2]);
        }
    }
}
