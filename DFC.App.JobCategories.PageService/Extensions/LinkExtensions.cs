using DFC.App.JobCategories.Data.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace DFC.App.JobCategories.PageService.Extensions
{
    public static class LinkExtensions
    {
        public static T GetId<T>(this DynamicLink value)
        {
            if (value == null)
            {
                throw new InvalidOperationException($"Value passed to {nameof(GetId)} is null");
            }

            var returnValue = value.Href!.Segments.Last().TrimEnd('/');

            if (typeof(T) == typeof(string))
            {
                return (T)(object)returnValue;
            }

            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)Guid.Parse(returnValue);
            }

            throw new InvalidOperationException($"{nameof(GetId)} does not support covnverting to {typeof(T)}");
        }
    }
}
