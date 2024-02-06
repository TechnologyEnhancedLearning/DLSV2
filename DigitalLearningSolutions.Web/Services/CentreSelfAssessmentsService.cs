namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using System.Collections.Generic;

    public interface ICentreSelfAssessmentsService
    {
        IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId);
        CentreSelfAssessment? GetCentreSelfAssessmentByCentreAndID(int centreId, int selfAssessmentId);
        IEnumerable<SelfAssessmentForPublish> GetCentreSelfAssessmentsForPublish(int centreId);
        void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId);
        void InsertCentreSelfAssessment(int centreId, int selfAssessmentId, bool selfEnrol);
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
            public IEnumerable<SelfAssessmentForPublish> GetCentreSelfAssessmentsForPublish(int centreId)
            {
                return centreSelfAssessmentsDataService.GetCentreSelfAssessmentsForPublish(centreId);
            }
            public void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId)
            {
                centreSelfAssessmentsDataService.DeleteCentreSelfAssessment(centreId, selfAssessmentId);
            }

            public void InsertCentreSelfAssessment(int centreId, int selfAssessmentId, bool selfEnrol)
            {
                centreSelfAssessmentsDataService.InsertCentreSelfAssessment(centreId, selfAssessmentId, selfEnrol);
            }
        }
    }

}
