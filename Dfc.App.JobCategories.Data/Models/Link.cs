using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.JobCategories.Data.Models.API
{
    [ExcludeFromCodeCoverage]
    public class Link
    {
        public KeyValuePair<string, DynamicLink> LinkValue { get; set; }
    }
}
