﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Dfc.App.JobCategories.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbConnection
    {
        public string AccessKey { get; set; }

        public Uri EndpointUrl { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }

        public string PartitionKey { get; set; }
    }
}