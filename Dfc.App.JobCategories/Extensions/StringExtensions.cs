using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Extensions
{
    public static class StringExtensions
    {
        public static string GetContentItemType(this string value)
        {
            return ConvertToUri(value).Segments[1];
        }

        public static Guid GetContentItemId(this string value)
        {
            return Guid.Parse(ConvertToUri(value).Segments[2]);
        }

        private static Uri ConvertToUri(this string value)
        {
            return new Uri("http://fakehost.net" + value);
        }
    }
}
