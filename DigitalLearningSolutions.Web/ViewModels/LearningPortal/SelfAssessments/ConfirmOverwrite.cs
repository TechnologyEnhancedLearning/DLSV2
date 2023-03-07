using DigitalLearningSolutions.Web.Attributes;
using System.ComponentModel;
namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class ConfirmOverwrite
    {
        public ConfirmOverwrite() { }
        public ConfirmOverwrite(
            int competencyId,
            int competencyNumber,
            int competencyGroupId,
            string competencyName,
            int selfAssessmentId
        )
        {
            CompetencyGroupId = competencyGroupId;
            CompetencyName = competencyName;
            CompetencyNumber = competencyNumber;
            CompetencyId = competencyId;
            SelfAssessmentId = selfAssessmentId;
        }

        public int SelfAssessmentId { get; set; }
        public int CompetencyGroupId { get; set; }
        public int CompetencyId { get; set; }
        public int CompetencyNumber { get; set; }
        public string CompetencyName { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "You must check the checkbox to continue")]
        [DefaultValue(false)]
        public bool IsChecked { get; set; }
    }
}
