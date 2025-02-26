using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.Collections.Generic;

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

        CompetencyAssessmentTaskStatus GetCompetencyAssessmentTaskStatus(int assessmentId, int? frameworkId);

        //UPDATE DATA
        bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName);

        bool UpdateCompetencyAssessmentProfessionalGroup(int competencyAssessmentId, int adminId, int? nrpProfessionalGroupID);
        bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateCompetencyAssessmentDescription(int assessmentId, int adminId, string description);
        bool UpdateCompetencyAssessmentBranding(int assessmentId, int adminId, int brandID, int categoryID);
        bool UpdateBrandingTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateCompetencyAssessmentVocabulary(int assessmentId, int adminId, string vocabulary);
        bool UpdateVocabularyTaskStatus(int assessmentId, bool taskStatus);

        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName, int? frameworkId);
        
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
                    competencyAssessmentDataService.UpdateCompetencyAssessmentBranding(assessmentId, (int)framework.BrandID, (int)framework.CategoryID, adminId);
                    competencyAssessmentDataService.UpdateCompetencyAssessmentVocabulary(assessmentId, adminId, framework.Vocabulary);
                }
            }
            return assessmentId;
        }

        public bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentName(competencyAssessmentId, adminId, competencyAssessmentName);
        }

        public bool UpdateCompetencyAssessmentProfessionalGroup(int competencyAssessmentId, int adminId, int? nrpProfessionalGroupID)
        {
            return competencyAssessmentDataService.UpdateCompetencyAssessmentProfessionalGroup(competencyAssessmentId, adminId, nrpProfessionalGroupID);
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
    }
}
