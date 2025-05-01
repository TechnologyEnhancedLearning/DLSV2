using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Attributes;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ConfirmRemoveFrameworkSourceViewModel
    {
        public ConfirmRemoveFrameworkSourceViewModel() { }
        public ConfirmRemoveFrameworkSourceViewModel(CompetencyAssessmentBase competencyAssessmentBase, DetailFramework framework, int competencyCount)
        {
            CompetencyCount = competencyCount;
            AssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            FrameworkName = framework.FrameworkName;
            FrameworkID = framework.ID;
        }
        public string? AssessmentName { get; set; }
        public string? FrameworkName { get; set; }
        public int FrameworkID { get; set; }
        public int CompetencyCount { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "You must confirm that you wish to remove this framework")]
        public bool Confirm { get; set; }

    }
}
