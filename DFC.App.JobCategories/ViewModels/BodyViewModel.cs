using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace DFC.App.JobCategories.ViewModels
{
    public class BodyViewModel
    {
        public HtmlString? Content { get; set; }

        public string Category { get; set; } = "Administration";

        public List<JobProfileListItemViewModel> Profiles { get; set; } = new List<JobProfileListItemViewModel>
        {
            new JobProfileListItemViewModel("Accounting technician", "/job-profiles/accounting-technician", null, "Accounting technicians handle day-to-day financial matters in all types of business."),
            new JobProfileListItemViewModel("Admin assistant", "/job-profiles/admin-assistant", "Office administrator, clerical assistant, administrative assistant", "Admin assistants give support to offices by organising meetings, typing documents and updating computer records."),
            new JobProfileListItemViewModel("Arts administrator", "/job-profiles/arts-administrator", null, "Arts administrators help to organise events and exhibitions, manage staff, and look after buildings like theatres or museums."),
        };
    }
}
