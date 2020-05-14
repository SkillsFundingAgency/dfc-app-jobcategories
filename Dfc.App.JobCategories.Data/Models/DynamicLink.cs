using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class DynamicLink
    {
        public Uri? Href { get; set; }

        public string? Relationship { get; set; }
    }
}
