﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class CompletedCourseViewModel : StartedLearningItemViewModel
    {
        public DateTime CompletedDate { get; }
        public DateTime? EvaluatedDate { get; }
        public string EvaluateUrl { get; }


        public CompletedCourseViewModel(CompletedCourse course, IConfiguration config) : base(course)
        {
            CompletedDate = course.Completed;
            EvaluatedDate = course.Evaluated;
            EvaluateUrl = config.GetEvaluateUrl(course.ProgressID);
        }

        public string FinaliseButtonText()
        {
            if (HasLearningContent && EvaluatedDate == null)
            {
                return "Evaluate";
            }

            if (HasLearningAssessmentAndCertification && (EvaluatedDate != null || !HasLearningContent))
            {
                return "Certificate";
            }

            return "";
        }

        public string FinaliseButtonAriaLabel()
        {
            return FinaliseButtonText() switch
            {
                "Evaluate" => "Evaluate course",
                "Certificate" => "View or print certificate",
                _ => ""
            };
        }

        public bool HasFinaliseButton()
        {
            return FinaliseButtonText() != "";
        }
    }
}
