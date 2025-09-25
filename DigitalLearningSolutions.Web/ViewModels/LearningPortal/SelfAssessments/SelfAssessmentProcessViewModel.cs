namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class SelfAssessmentProcessViewModel
    {
        public int SelfAssessmentID { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm that you understand and agree to the self-assessment process")]
        public bool ActionConfirmed { get; set; }

        public string? VocabPlural { get; set; }
        public string? Vocabulary { get; set; }

    }
}
