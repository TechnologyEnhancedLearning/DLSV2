namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportCompetenciesResultsViewModel
    {
        public ImportCompetenciesResultsViewModel(ImportCompetenciesResult importCompetenciesResult)
        {
            ProcessedCount = importCompetenciesResult.ProcessedCount;
            CompetenciesInsertedCount = importCompetenciesResult.CompetenciesInsertedCount;
            CompetencyGroupsInsertedCount = importCompetenciesResult.CompetencyGroupsInsertedCount;
            SkippedCount = importCompetenciesResult.SkippedCount;
            Errors = importCompetenciesResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
        }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ProcessedCount { get; set; }
        public int CompetenciesInsertedCount { get; set; }
        public int CompetencyGroupsInsertedCount { get; set; }
        public int SkippedCount { get; set; }
        private string MapReasonToErrorMessage(ImportCompetenciesResult.ErrorReason reason)
        {
            return reason switch
            {
                ImportCompetenciesResult.ErrorReason.MissingCompetencyName =>
                    "Competency name was not provided. Competency name is a required field and cannot be left blank",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyName => "Competency name must be 500 characters or fewer",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName => "Competency group name must be 255 characters or fewer",
            };
        }
    }
}
