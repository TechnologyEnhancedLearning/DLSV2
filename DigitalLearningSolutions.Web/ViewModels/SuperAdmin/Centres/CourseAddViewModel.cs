namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using System;
    using System.Collections.Generic;

    public class CourseAddViewModel
    {
        public string? SearchTerm { get; set; }
        public int CentreId { get; set; }
        public string CourseType { get; set; }
        public string CentreName { get; set; }
        public IEnumerable<CourseForPublish> Courses { get; set; }
        public List<int>? ApplicationIds { get; set; }
    }
}
