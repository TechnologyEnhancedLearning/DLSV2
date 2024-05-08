namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SelfAssessmentAddViewModel
    {
        public int CentreId { get; set; }
        public string? CentreName { get; set; }
        public IEnumerable<SelfAssessmentForPublish>? SelfAssessments { get; set; }
        [Required(ErrorMessage = "Please select at least one self assessment")]
        public List<int> SelfAssessmentIds { get; set; }
        [Required(ErrorMessage = "Please indicate whether learners will be allowed to self enrol on the self assessment(s)")]
        public bool? EnableSelfEnrolment { get; set; }
    }
}
