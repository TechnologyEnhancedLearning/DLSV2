namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Configuration;

    public class CompletedCourseViewModel : CompletedLearningItemViewModel
    {
        public CompletedCourseViewModel(CompletedLearningItem course, IConfiguration config) : base(course)
        {
            EvaluateUrl = config.GetEvaluateUrl(course.ProgressID, true);
        }

        public string EvaluateUrl { get; }

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
                _ => "",
            };
        }

        public bool HasFinaliseButton()
        {
            return FinaliseButtonText() != "";
        }
    }
}
