namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using System.Collections.Generic;

    public interface ICentreSelfAssessmentsService
    {
        IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId);
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
        }
    }

}
