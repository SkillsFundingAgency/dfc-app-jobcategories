﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobCategories.Data.Models
{
    public class ServiceTaxonomyApiClientOptions
    {
        public Uri? BaseAddress { get; set; }

        public string Endpoint { get; set; } = "api/Execute/{0}/{1}";

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);
    }
}
