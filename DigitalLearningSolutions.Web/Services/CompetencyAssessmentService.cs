using DigitalLearningSolutions.Data.DataServices;
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

        CompetencyAssessmentBase? GetCompetencyAssessmentByName(string competencyAssessmentName, int adminId);

        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();

        CompetencyAssessmentTaskStatus GetCompetencyAssessmentTaskStatus(int assessmentId, int? frameworkId);

        //UPDATE DATA
        bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName);

        bool UpdateCompetencyAssessmentProfessionalGroup(int competencyAssessmentId, int adminId, int? nrpProfessionalGroupID);
        bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus);

        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName, int? frameworkId);
        bool UpdateCompetencyAssessmentDescription(int iD, int adminId, string description);
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

        public CompetencyAssessmentBase? GetCompetencyAssessmentByName(string competencyAssessmentName, int adminId)
        {
            return competencyAssessmentDataService.GetCompetencyAssessmentByName(competencyAssessmentName, adminId);
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
    }
}
