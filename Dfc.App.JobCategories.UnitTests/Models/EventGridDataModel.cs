using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.Models
{
    public class EventGridDataModel
    {
        public Uri? Api { get; set; }
        public string? VersionId { get; set; }
        public string? DisplayText { get; set; }
        public string? Author { get; set; }
    }
}
