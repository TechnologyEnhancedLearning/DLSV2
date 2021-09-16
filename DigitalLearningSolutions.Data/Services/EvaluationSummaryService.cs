namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IEvaluationSummaryService
    {
        EvaluationSummaryData GetEvaluationSummaryData(int centreId, ActivityFilterData filterData);
    }

    public class EvaluationSummaryService : IEvaluationSummaryService
    {
        private readonly IEvaluationSummaryDataService evaluationSummaryDataService;

        public EvaluationSummaryService(IEvaluationSummaryDataService evaluationSummaryDataService)
        {
            this.evaluationSummaryDataService = evaluationSummaryDataService;
        }

        public EvaluationSummaryData GetEvaluationSummaryData(int centreId, ActivityFilterData filterData)
        {
            return evaluationSummaryDataService
                .GetEvaluationSummaryData(
                    centreId,
                    filterData.StartDate,
                    filterData.EndDate,
                    filterData.JobGroupId,
                    filterData.CourseCategoryId,
                    filterData.CustomisationId
                );
        }
    }
}
