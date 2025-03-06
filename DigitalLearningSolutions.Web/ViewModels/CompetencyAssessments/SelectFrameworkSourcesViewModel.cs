using AngleSharp.Css;
using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectFrameworkSourcesViewModel
    {
        public SelectFrameworkSourcesViewModel() { }
        public SelectFrameworkSourcesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<BrandedFramework> frameworks, int[] selectedFrameworks, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
            Frameworks = frameworks;
            SelectedFrameworks = selectedFrameworks;
        }
        public IEnumerable<BrandedFramework> Frameworks { get; set; }
        public int[] SelectedFrameworks { get; set; }
        public IEnumerable<NRPRoles> Roles { get; set; }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public bool? TaskStatus { get; set; }
        public string? GroupName { get; set; }
        public string? SubGroupName { get; set; }
        public string? RoleName { get; set; }
    }
}
