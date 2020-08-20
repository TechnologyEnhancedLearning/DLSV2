namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class CompletedCourseViewModel : BaseCourseViewModel
    {
        public DateTime CompletedDate { get; }
        public DateTime? EvaluatedDate { get; }
        public string EvaluateUrl { get; }


        public CompletedCourseViewModel(CompletedCourse course, IConfiguration config) : base(course, config)
        {
            CompletedDate = course.Completed;
            EvaluatedDate = course.Evaluated;
            EvaluateUrl = config.GetEvaluateUrl(course.ProgressID);
        }
    }
}
