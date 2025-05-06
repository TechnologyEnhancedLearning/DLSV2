using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectFrameworkSourcesViewModel : SelectFrameworkSourcesFormData
    {
        public SelectFrameworkSourcesViewModel() { }
        public SelectFrameworkSourcesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<BrandedFramework> frameworks, int[] additionalFrameworksIds, int? primaryFramework, bool? taskStatus, string actionName)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
            PrimaryFramework = frameworks.FirstOrDefault(f => f.ID == primaryFramework);
            var excludedIds = new HashSet<int>(additionalFrameworksIds);
            if (primaryFramework.HasValue)
            {
                excludedIds.Add(primaryFramework.Value);
            }
            Frameworks = [.. frameworks
                .Where(f => !excludedIds.Contains(f.ID))
                .OrderBy(f => f.FrameworkName)];
            AdditionalFrameworks = [.. additionalFrameworksIds.Select(id => frameworks.First(f => f.ID == id))];
            ActionName = actionName;
        }
        public IEnumerable<BrandedFramework> Frameworks { get; set; }
        public IEnumerable<BrandedFramework> AdditionalFrameworks { get; set; }
        public BrandedFramework? PrimaryFramework { get; set; }
        public IEnumerable<NRPRoles> Roles { get; set; }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string? GroupName { get; set; }
        public string? SubGroupName { get; set; }
        public string? RoleName { get; set; }
    }
}
