using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICompetencyAssessmentService
    {
        //GET DATA
        IEnumerable<CompetencyAssessment> GetAllCompetencyAssessments(int adminId);

        IEnumerable<CompetencyAssessment> GetCompetencyAssessmentsForAdminId(int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentBaseById(int competencyAssessmentId, int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentBaseByName(string competencyAssessmentName, int adminId);
        CompetencyAssessment? GetCompetencyAssessmentById(int competencyAssessmentId, int adminId);

        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();
        IEnumerable<NRPSubGroups> GetNRPSubGroups(int? nRPProfessionalGroupID);
        IEnumerable<NRPRoles> GetNRPRoles(int? nRPSubGroupID);

        CompetencyAssessmentTaskStatus GetCompetencyAssessmentTaskStatus(int assessmentId, int? frameworkId);
        int[] GetLinkedFrameworkIds(int assessmentId);
        int? GetPrimaryLinkedFrameworkId(int assessmentId);

        IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId);
        IEnumerable<LinkedFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId);

        bool RemoveSelfAssessmentFramework(int assessmentId, int frameworkId, int adminId);

        int[] GetLinkedFrameworkCompetencyIds(int competencyAssessmentId, int frameworkId);
        CompetencyAssessmentFeatures? GetCompetencyAssessmentFeaturesTaskStatus(int competencyAssessmentId);
        int? GetSelfAssessmentStructure(int competencyAssessmentId);
        List<GroupedCompetencyWithAssessmentRoleRequirements> GetGroupedCompetencyWithAssessmentRoleRequirements(int competencyAssessmentId, int? competencyId, int? assessmentQuestionId);
        int GetCountOfAsssessmentQuestionInCompetencyAssessment(int competencyAssessmentId, int assessmentQuestionId);
        //UPDATE DATA
        bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName);
        bool UpdateCompetencyRoleProfileLinks(int competencyAssessmentId, int adminId, int? professionalGroupId, int? subGroupId, int? roleId);
        bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateCompetencyAssessmentDescription(int assessmentId, int adminId, string description);
        bool UpdateCompetencyAssessmentBranding(int assessmentId, int adminId, int brandID, int categoryID);
        bool UpdateBrandingTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateCompetencyAssessmentVocabulary(int assessmentId, int adminId, string vocabulary);
        bool UpdateVocabularyTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateRoleProfileLinksTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateSelectCompetenciesTaskStatus(int competencyAssessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateOptionalCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateWorkingGroupTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateCompetencyAssessmentRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateCompetencyAssessmentOptions(
            bool includeLearnerDeclarationPrompt,
            bool includesSignposting,
            bool linearNavigation,
            bool useDescriptionExpanders,
            string? questionLabelText,
            string? reviewerCommentsLabelText,
            int competencyAssessmentId, int adminId);
        bool UpdateCompetencyAssessmentOptionsTaskStatus(int assessmentId, bool taskStatus);
        void MoveCompetencyInSelfAssessment(int competencyAssessmentId,
            int competencyId,
            string direction
        );
        void MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId,
            int groupId,
            string direction
        );
        bool UpdateCompetencyAssessmentFeaturesTaskStatus(int id, bool descriptionStatus, bool providerandCategoryStatus, bool vocabularyStatus,
          bool workingGroupStatus, bool AllframeworkCompetenciesStatus);
        void UpdateSelfAssessmentFromFramework(int selfAssessmentId, int? frameworkId);
        bool UpdateOptionalCompetenciesInAssessment(int selfAssessmentId, int[] groupIds, int[] selectedStructureIds);
        void UpdateMinimumOptionalCompetencies(int selfAssessmentId, int minimumOptionalCompetecies);
        void UpdateManageOptionalCompetenciesPrompt(int selfAssessmentId, string? manageOptionalCompetenciesPrompt);
        bool UpdatePrimaryFrameworkCompetencies(int assessmentId, int frameworkId);
        void UpdateRoleRequirementsFlags(int assessmentId, bool enforceRoleRequirementsForSignOff, bool includeRequirementsFilters);
        int UpdateAssessmentQuestionRoleRequirementsForSelfAssessment(int assessmentId, int assessmentQuestionId, Dictionary<int, int?> responseRoleRequirements);
        int UpdateCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int competencyId, int assessmentQuestionId, Dictionary<int, int?> responseRoleRequirements);
        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName, int? frameworkId);
        bool InsertSelfAssessmentFramework(int adminId, int assessmentId, int frameworkId);
        int GetCompetencyCountByFrameworkId(int competencyAssessmentId, int frameworkId);
        bool InsertCompetenciesIntoAssessmentFromFramework(int[] selectedCompetencyIds, int frameworkId, int competencyAssessmentId);
        bool InsertSelfAssessmentStructure(int selfAssessmentId, int? frameworkId);
        //DELETE DATA
        bool RemoveFrameworkCompetenciesFromAssessment(int competencyAssessmentId, int frameworkId);
        bool RemoveCompetencyFromAssessment(int competencyAssessmentId, int competencyId);
        bool RemoveCompetencyGroupFromAssessment(int competencyAssessmentId, int competencyGroupId);
        IEnumerable<CompetencyAssessmentCollaboratorDetail> GetCollaboratorsForCompetencyAssessmentId(int competencyAssessmentId);
        int AddCollaboratorToCompetencyAssessment(int competencyAssessmentId, string? userEmail, bool canModify, int? centreID);
        void RemoveCollaboratorFromCompetencyAssessment(int competencyAssessmentId, int id);
        CompetencyAssessmentCollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId);
        bool HasCompetencyWithSignpostedLearning(int competencyAssessmentId);
    }
    public class CompetencyAssessmentService : ICompetencyAssessmentService
    {
        private readonly ICompetencyAssessmentDataService competencyAssessmentDataService;
        private readonly IFrameworkDataService frameworkDataService;
        public CompetencyAssessmentService(ICompetencyAssessmentDataService competencyAssessmentDataService, IFrameworkDataService frameworkDataService)
        {
            this.competencyAssessmentDataService = competencyAssessmentDataService;
            this.frameworkDataService = frameworkDataService;
        }
        public IEnumerable<CompetencyAssessment> GetAllCompetencyAssessments(int adminId)
        {
            return competencyAssessmentDataService.GetAllCompetencyAssessments(adminId);
        }

        public IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups()
        {
            return competencyAssessmentDataService.GetNRPProfessionalGroups();
        }

        public CompetencyAssessmentBase? GetCompetencyAssessmentBaseById(int competencyAssessmentId, int adminId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
        }

        public CompetencyAssessmentBase? GetCompetencyAssessmentBaseByName(string competencyAssessmentName, int adminId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentBaseByName(competencyAssessmentName, adminId);
        }

        public IEnumerable<CompetencyAssessment> GetCompetencyAssessmentsForAdminId(int adminId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentsForAdminId(adminId);
        }
        public int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName, int? frameworkId)
        {
            var assessmentId = competencyAssessmentDataService.InsertCompetencyAssessment(adminId, centreId, competencyAssessmentName);
            if (assessmentId > 0 && frameworkId != null)
            {
                var framework = frameworkDataService.GetBrandedFrameworkByFrameworkId((int)frameworkId, adminId);
                if (framework != null)
                {
                    competencyAssessmentDataService.InsertSelfAssessmentFramework(adminId, assessmentId, framework.ID);
                    competencyAssessmentDataService.UpdateCompetencyAssessmentDescription(adminId, assessmentId, framework.Description);
                    competencyAssessmentDataService.UpdateCompetencyAssessmentBranding(assessmentId, adminId, (int)framework.BrandID, (int)framework.CategoryID);
                    competencyAssessmentDataService.UpdateCompetencyAssessmentVocabulary(assessmentId, adminId, framework.Vocabulary);
                }
            }
            return assessmentId;
        }

        public bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentName(competencyAssessmentId, adminId, competencyAssessmentName);
        }

        public bool UpdateCompetencyRoleProfileLinks(int competencyAssessmentId, int adminId, int? professionalGroupId, int? subGroupId, int? roleId)
        {
            return competencyAssessmentDataService.UpdateCompetencyRoleProfileLinks(competencyAssessmentId, adminId, professionalGroupId, subGroupId, roleId);
        }
        public CompetencyAssessmentTaskStatus GetCompetencyAssessmentTaskStatus(int assessmentId, int? frameworkId)
        {
            return competencyAssessmentDataService.GetOrInsertAndReturnAssessmentTaskStatus(assessmentId, (frameworkId != null));
        }
        public bool UpdateCompetencyAssessmentDescription(int competencyAssessmentId, int adminId, string description)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentDescription(competencyAssessmentId, adminId, description);
        }
        public bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateIntroductoryTextTaskStatus(assessmentId, taskStatus);
        }

        public CompetencyAssessment? GetCompetencyAssessmentById(int competencyAssessmentId, int adminId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentById(competencyAssessmentId, adminId);
        }

        public bool UpdateCompetencyAssessmentBranding(int assessmentId, int adminId, int brandID, int categoryID)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentBranding(assessmentId, adminId, brandID, categoryID);
        }

        public bool UpdateBrandingTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateBrandingTaskStatus(assessmentId, taskStatus);
        }

        bool ICompetencyAssessmentService.UpdateCompetencyAssessmentVocabulary(int assessmentId, int adminId, string vocabulary)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentVocabulary(assessmentId, adminId, vocabulary);
        }

        bool ICompetencyAssessmentService.UpdateVocabularyTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateVocabularyTaskStatus(assessmentId, taskStatus);
        }

        public IEnumerable<NRPSubGroups> GetNRPSubGroups(int? nRPProfessionalGroupID)
        {
            return competencyAssessmentDataService.GetNRPSubGroups(nRPProfessionalGroupID);
        }

        public IEnumerable<NRPRoles> GetNRPRoles(int? nRPSubGroupID)
        {
            return competencyAssessmentDataService.GetNRPRoles(nRPSubGroupID);
        }

        public bool UpdateRoleProfileLinksTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateRoleProfileLinksTaskStatus(assessmentId, taskStatus);
        }

        public int[] GetLinkedFrameworkIds(int assessmentId)
        {
            return competencyAssessmentDataService.GetLinkedFrameworkIds(assessmentId);
        }

        public bool InsertSelfAssessmentFramework(int adminId, int assessmentId, int frameworkId)
        {
            return competencyAssessmentDataService.InsertSelfAssessmentFramework(adminId, assessmentId, frameworkId);
        }

        public bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateFrameworkLinksTaskStatus(assessmentId, taskStatus, previousStatus);
        }

        public int? GetPrimaryLinkedFrameworkId(int assessmentId)
        {
            return competencyAssessmentDataService.GetPrimaryLinkedFrameworkId(assessmentId);
        }

        public bool RemoveSelfAssessmentFramework(int assessmentId, int frameworkId, int adminId)
        {
           return competencyAssessmentDataService.RemoveSelfAssessmentFramework(assessmentId, frameworkId, adminId);
        }

        public int GetCompetencyCountByFrameworkId(int competencyAssessmentId, int frameworkId)
        {
            return competencyAssessmentDataService.GetCompetencyCountByFrameworkId(competencyAssessmentId, frameworkId);
        }

        public bool RemoveFrameworkCompetenciesFromAssessment(int competencyAssessmentId, int frameworkId)
        {
            UpdateSelectCompetenciesTaskStatus(competencyAssessmentId, false, true);
            UpdateOptionalCompetenciesTaskStatus(competencyAssessmentId, false, true);
            UpdateRoleRequirementsTaskStatus(competencyAssessmentId, false, true);
            return competencyAssessmentDataService.RemoveFrameworkCompetenciesFromAssessment(competencyAssessmentId, frameworkId);
        }

        public bool UpdateSelectCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateSelectCompetenciesTaskStatus(assessmentId, taskStatus, previousStatus);
        }

        public bool UpdateOptionalCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateOptionalCompetenciesTaskStatus(assessmentId, taskStatus, previousStatus);
        }

        public bool UpdateRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateRoleRequirementsTaskStatus(assessmentId, taskStatus, previousStatus);
        }
        public bool UpdateWorkingGroupTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateWorkingGroupTaskStatus(assessmentId, taskStatus, previousStatus);
        }
        public bool UpdateCompetencyAssessmentOptions(
            bool includeLearnerDeclarationPrompt,
            bool includesSignposting,
            bool linearNavigation,
            bool useDescriptionExpanders,
            string? questionLabelText,
            string? reviewerCommentsLabelText,
            int competencyAssessmentId, int adminId)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentOptions(
            includeLearnerDeclarationPrompt,
            includesSignposting,
            linearNavigation,
            useDescriptionExpanders,
            questionLabelText,
            reviewerCommentsLabelText,
            competencyAssessmentId, adminId);
        }
        public IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
        }

        public IEnumerable<LinkedFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.GetLinkedFrameworksForCompetencyAssessment(competencyAssessmentId);
        }

        public int[] GetLinkedFrameworkCompetencyIds(int competencyAssessmentId, int frameworkId)
        {
            return competencyAssessmentDataService.GetLinkedFrameworkCompetencyIds(competencyAssessmentId, frameworkId);
        }

        public bool InsertCompetenciesIntoAssessmentFromFramework(int[] selectedCompetencyIds, int frameworkId, int competencyAssessmentId)
        {
            return competencyAssessmentDataService.InsertCompetenciesIntoAssessmentFromFramework(selectedCompetencyIds, frameworkId, competencyAssessmentId);
        }

        public bool RemoveCompetencyFromAssessment(int competencyAssessmentId, int competencyId)
        {
            return competencyAssessmentDataService.RemoveCompetencyFromAssessment(competencyAssessmentId, competencyId);
        }
        public bool RemoveCompetencyGroupFromAssessment(int competencyAssessmentId, int competencyGroupId)
        {
            return competencyAssessmentDataService.RemoveCompetencyGroupFromAssessment(competencyAssessmentId, competencyGroupId);
        }

        public void MoveCompetencyInSelfAssessment(int competencyAssessmentId, int competencyId, string direction)
        {
            competencyAssessmentDataService.MoveCompetencyInSelfAssessment(competencyAssessmentId, competencyId, direction);
        }

        public void MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId, int groupId, string direction)
        {
            competencyAssessmentDataService.MoveCompetencyGroupInSelfAssessment(competencyAssessmentId, groupId, direction);
        }
        public bool UpdateCompetencyAssessmentFeaturesTaskStatus(int id, bool descriptionStatus, bool providerandCategoryStatus, bool vocabularyStatus,
          bool workingGroupStatus, bool AllframeworkCompetenciesStatus)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentFeaturesTaskStatus(id, descriptionStatus, providerandCategoryStatus, vocabularyStatus,
            workingGroupStatus, AllframeworkCompetenciesStatus);
        }
        public bool UpdateCompetencyAssessmentRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentRoleRequirementsTaskStatus(assessmentId, taskStatus);
        }
        public CompetencyAssessmentFeatures? GetCompetencyAssessmentFeaturesTaskStatus(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentFeaturesTaskStatus(competencyAssessmentId);
        }
        public bool InsertSelfAssessmentStructure(int selfAssessmentId, int? frameworkId)
        {
            return competencyAssessmentDataService.InsertSelfAssessmentStructure(selfAssessmentId, frameworkId);
        }
        public void UpdateSelfAssessmentFromFramework(int selfAssessmentId, int? frameworkId)
        {
            competencyAssessmentDataService.UpdateSelfAssessmentFromFramework(selfAssessmentId, frameworkId);
        }
        public bool UpdatePrimaryFrameworkCompetencies(int assessmentId, int frameworkId)
        {
            return competencyAssessmentDataService.UpdatePrimaryFrameworkCompetencies(assessmentId, frameworkId);
        }

        public int? GetSelfAssessmentStructure(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.GetSelfAssessmentStructure(competencyAssessmentId);
        }

        public bool UpdateOptionalCompetenciesInAssessment(int selfAssessmentId, int[] groupIds, int[] selectedStructureIds)
        {
            return competencyAssessmentDataService.UpdateOptionalCompetenciesInAssessment(selfAssessmentId, groupIds, selectedStructureIds);
        }

        public void UpdateMinimumOptionalCompetencies(int selfAssessmentId, int minimumOptionalCompetecies)
        {
            competencyAssessmentDataService.UpdateMinimumOptionalCompetencies(selfAssessmentId, minimumOptionalCompetecies);
        }
        public void UpdateManageOptionalCompetenciesPrompt(int selfAssessmentId, string? manageOptionalCompetenciesPrompt)
        {
            manageOptionalCompetenciesPrompt = SanitizerHelper.SanitizeHtmlData(manageOptionalCompetenciesPrompt);
            if (StringHelper.StripHtmlTags(manageOptionalCompetenciesPrompt) == "")
            {
                manageOptionalCompetenciesPrompt = null;
            }
            competencyAssessmentDataService.UpdateManageOptionalCompetenciesPrompt(selfAssessmentId, manageOptionalCompetenciesPrompt);
        }
        public IEnumerable<CompetencyAssessmentCollaboratorDetail> GetCollaboratorsForCompetencyAssessmentId(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.GetCollaboratorsForCompetencyAssessmentId(competencyAssessmentId);
        }
        public int AddCollaboratorToCompetencyAssessment(int competencyAssessmentId, string? userEmail, bool canModify, int? centreID)
        {
            return competencyAssessmentDataService.AddCollaboratorToCompetencyAssessment(competencyAssessmentId, userEmail, canModify, centreID);
        }
        public void RemoveCollaboratorFromCompetencyAssessment(int competencyAssessmentId, int id)
        {
            competencyAssessmentDataService.RemoveCollaboratorFromCompetencyAssessment(competencyAssessmentId, id);
        }
        public CompetencyAssessmentCollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId)
        {
            return competencyAssessmentDataService.GetCollaboratorNotification(id, invitedByAdminId);

        }
        public bool HasCompetencyWithSignpostedLearning(int competencyAssessmentId)
        {
            return competencyAssessmentDataService.HasCompetencyWithSignpostedLearning(competencyAssessmentId);
        }
        public bool UpdateCompetencyAssessmentOptionsTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentOptionsTaskStatus(assessmentId, taskStatus);
        }

        public List<GroupedCompetencyWithAssessmentRoleRequirements> GetGroupedCompetencyWithAssessmentRoleRequirements(int competencyAssessmentId, int? competencyId, int? assessmentQuestionId)
        {
            var competencyWithAssessmentQuestionRoleRequirements = competencyAssessmentDataService.GetCompetencyWithAssessmentQuestionRoleRequirements(competencyAssessmentId, competencyId, assessmentQuestionId).ToList();
            return [.. competencyWithAssessmentQuestionRoleRequirements
       .GroupBy(x => new
       {
           x.CompetencyGroupID,
           x.GroupName
       })
       .Select(group => new GroupedCompetencyWithAssessmentRoleRequirements
       {
           CompetencyGroupID = group.Key.CompetencyGroupID ?? 0,
           GroupName = group.Key.GroupName ?? "Ungrouped",

           Competencies = [.. group
               .GroupBy(c => new
               {
                   c.CompetencyID,
                   c.Competency,
                   c.CompetencyDescription,
                   c.Optional
               })
               .Select(comp => new CompetencyModel
               {
                   CompetencyID = comp.Key.CompetencyID,
                   Name = comp.Key.Competency,
                   Description = comp.Key.CompetencyDescription,
                   Optional = comp.Key.Optional,

                   Questions = [.. comp
    .GroupBy(q => new
    {
        q.AssessmentQuestionID,
        q.Question,
        q.InputTypeName,
        q.Required
    })
    .Select(q => new AssessmentQuestionModel
    {
        AssessmentQuestionID = q.Key.AssessmentQuestionID,
        Question = q.Key.Question,
        InputTypeName = q.Key.InputTypeName,
        Required = q.Key.Required,
        Responses = [.. q
            .Where(r => r.ResponseValue.HasValue)
            .Select(r => new ResponseModel
            {
                ResponseValue = r.ResponseValue,
                ResponseLabel = r.Response,
                LevelRAG = r.LevelRAG
            })
            .DistinctBy(r => r.ResponseValue)]
    })]
               })]
       })];
        }

        public void UpdateRoleRequirementsFlags(int assessmentId, bool enforceRoleRequirementsForSignOff, bool includeRequirementsFilters)
        {
            competencyAssessmentDataService.UpdateRoleRequirementsFlags(assessmentId, enforceRoleRequirementsForSignOff, includeRequirementsFilters);
        }

        public int GetCountOfAsssessmentQuestionInCompetencyAssessment(int competencyAssessmentId, int assessmentQuestionId)
        {
            return competencyAssessmentDataService.GetCountOfAsssessmentQuestionInCompetencyAssessment(competencyAssessmentId, assessmentQuestionId);
        }

        public int UpdateAssessmentQuestionRoleRequirementsForSelfAssessment(int assessmentId, int assessmentQuestionId, Dictionary<int, int?> responseRoleRequirements)
        {
            int rowCount = 0;
            foreach (var responseRoleRequirement in responseRoleRequirements)
            {
                competencyAssessmentDataService.DeleteCompetencyAssessmentQuestionRoleRequirement(assessmentId, null, assessmentQuestionId, responseRoleRequirement.Key);
                if (responseRoleRequirement.Value != null)
                {
                    rowCount += competencyAssessmentDataService.InsertAssessmentQuestionRoleRequirementForSelfAssessment(assessmentId, assessmentQuestionId, responseRoleRequirement.Key, responseRoleRequirement.Value);
                }
            }
            return rowCount;
        }

        public int UpdateCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int competencyId, int assessmentQuestionId, Dictionary<int, int?> responseRoleRequirements)
        {
            int rowCount = 0;
            foreach (var responseRoleRequirement in responseRoleRequirements)
            {
                competencyAssessmentDataService.DeleteCompetencyAssessmentQuestionRoleRequirement(assessmentId, competencyId, assessmentQuestionId, responseRoleRequirement.Key);
                if (responseRoleRequirement.Value != null)
                {
                    rowCount += competencyAssessmentDataService.InsertCompetencyAssessmentQuestionRoleRequirement(assessmentId, competencyId, assessmentQuestionId, responseRoleRequirement.Key, responseRoleRequirement.Value);
                }
            }
            return rowCount;
        }
    }
}
