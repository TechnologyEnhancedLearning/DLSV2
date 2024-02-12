namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using System.Collections.Generic;

    public interface ICentreSelfAssessmentsService
    {
        IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId);
        CentreSelfAssessment? GetCentreSelfAssessmentByCentreAndID(int centreId, int selfAssessmentId);
        void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId);
        public class CentreSelfAssessmentsService : ICentreSelfAssessmentsService
        {
            private readonly ICentreSelfAssessmentsDataService centreSelfAssessmentsDataService;
            public CentreSelfAssessmentsService(
                ICentreSelfAssessmentsDataService centreSelfAssessmentsDataService
        )
            {
                this.centreSelfAssessmentsDataService = centreSelfAssessmentsDataService;
            }

            public IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId)
            {
                return centreSelfAssessmentsDataService.GetCentreSelfAssessments(centreId);
            }
            public CentreSelfAssessment? GetCentreSelfAssessmentByCentreAndID(int centreId, int selfAssessmentId)
            {
                return centreSelfAssessmentsDataService.GetCentreSelfAssessmentByCentreAndID(centreId, selfAssessmentId);
            }
            public void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId)
            {
                centreSelfAssessmentsDataService.DeleteCentreSelfAssessment(centreId, selfAssessmentId);
            }


        }
    }

}
