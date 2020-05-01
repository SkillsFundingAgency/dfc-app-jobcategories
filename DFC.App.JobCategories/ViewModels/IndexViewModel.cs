using System.Collections.Generic;

namespace DFC.App.JobCategories.ViewModels
{
    public class IndexViewModel
    {
        public string? LocalPath { get; set; }

        public IEnumerable<IndexDocumentViewModel>? Documents { get; set; }
    }
}
