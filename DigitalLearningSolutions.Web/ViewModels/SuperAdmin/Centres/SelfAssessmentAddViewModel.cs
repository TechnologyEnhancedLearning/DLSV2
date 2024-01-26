namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;

    public class SelfAssessmentAddViewModel
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public IEnumerable<SelfAssessmentForPublish> SelfAssessments { get; set; }
        public int SelfAssessmentId { get; set; }
    }
}
