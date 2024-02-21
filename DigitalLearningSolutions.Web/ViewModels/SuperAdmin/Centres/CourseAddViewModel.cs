namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CourseAddViewModel
    {
        public string? SearchTerm { get; set; }
        public int CentreId { get; set; }
        public string CourseType { get; set; }
        public string? CentreName { get; set; }
        public IEnumerable<CourseForPublish>? Courses { get; set; }
        [Required(ErrorMessage = "Please select at least one course")]
        public List<int>? ApplicationIds { get; set; }
    }
}
