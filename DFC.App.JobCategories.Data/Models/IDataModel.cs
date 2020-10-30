using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobCategories.Data.Models
{
    public interface IDataModel
    {
        [Required]
        string? CanonicalName { get; }

        [Required]
        Uri? Uri { get; set; }

        [Required]
        DateTime DateModified { get; set; }
    }
}
