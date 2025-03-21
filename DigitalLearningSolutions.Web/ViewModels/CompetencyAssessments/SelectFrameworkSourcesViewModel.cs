using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectFrameworkSourcesViewModel : SelectFrameworkSourcesFormData
    {
        public SelectFrameworkSourcesViewModel() { }
        public SelectFrameworkSourcesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<BrandedFramework> frameworks, int[] selectedFrameworksIds, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
            Frameworks = frameworks.OrderBy(f => f.FrameworkName);
            SelectedFrameworks = [.. frameworks.Where(f => selectedFrameworksIds.Contains(f.ID))];
        }
        public IEnumerable<BrandedFramework> Frameworks { get; set; }
        public IEnumerable<BrandedFramework> SelectedFrameworks { get; set; }
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
