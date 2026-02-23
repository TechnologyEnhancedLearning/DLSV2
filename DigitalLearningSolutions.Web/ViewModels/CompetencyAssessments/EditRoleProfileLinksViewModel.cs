namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using Humanizer;
    using System.Collections.Generic;
    using System.Linq;

    public class EditRoleProfileLinksViewModel
    {
        public EditRoleProfileLinksViewModel() { }
        public EditRoleProfileLinksViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<NRPProfessionalGroups> professionalGroups, IEnumerable<NRPSubGroups> subGroups, IEnumerable<NRPRoles> roles, string actionName, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            ProfessionalGroupId = competencyAssessmentBase.NRPProfessionalGroupID;
            SubGroupId = competencyAssessmentBase.NRPSubGroupID;
            RoleId = competencyAssessmentBase.NRPRoleID;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
            ProfessionalGroups = professionalGroups;
            SubGroups = subGroups;
            Roles = roles;
            ActionName = actionName;
            RoleName = Roles.FirstOrDefault(r => r.ID == RoleId)?.ProfileName ?? "Don't link assessment to a role.";
            SubGroupName = SubGroups.FirstOrDefault(s => s.ID == SubGroupId)?.SubGroup ?? "Don't link assessment to a sub group.";
            GroupName = ProfessionalGroups.FirstOrDefault(p => p.ID == ProfessionalGroupId)?.ProfessionalGroup ?? "Don't link assessment to a professional group.";
        }
        public IEnumerable<NRPProfessionalGroups> ProfessionalGroups { get; set; }
        public IEnumerable<NRPSubGroups> SubGroups { get; set; }
        public IEnumerable<NRPRoles> Roles { get; set; }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public string ActionName { get; set; }
        public int? ProfessionalGroupId { get; set; }
        public int? SubGroupId { get; set; }
        public int? RoleId { get; set; }
        public int UserRole { get; set; }
        public bool? TaskStatus { get; set; }
        public string? GroupName { get; set; }
        public string? SubGroupName { get; set; }
        public string? RoleName { get; set; }
    }
}
