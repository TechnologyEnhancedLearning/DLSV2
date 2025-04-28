using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus);

        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName, int? frameworkId);
        bool InsertSelfAssessmentFramework(int adminId, int assessmentId, int frameworkId);
        int[] GetLinkedFrameworkIds(int assessmentId);
        
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

        public bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus)
        {
            return competencyAssessmentDataService.UpdateFrameworkLinksTaskStatus(assessmentId, taskStatus);
        }
    }
}
