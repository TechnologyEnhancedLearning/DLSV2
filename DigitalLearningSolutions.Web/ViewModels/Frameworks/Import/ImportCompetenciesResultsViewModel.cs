namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportCompetenciesResultsViewModel
    {
        public ImportCompetenciesResultsViewModel(ImportCompetenciesResult importCompetenciesResult, int frameworkId, string frameworkName, string frameworkVocabulary)
        {
            ProcessedCount = importCompetenciesResult.ProcessedCount;
            CompetenciesInsertedCount = importCompetenciesResult.CompetencyAddedCount;
            CompetenciesUpdatedCount = importCompetenciesResult.CompetencyUpdatedCount;
            CompetencyGroupsInsertedCount = importCompetenciesResult.GroupAddedCount;
            CompetencyGroupsUpdatedCount = importCompetenciesResult.GroupUpdatedCount;
            CompetenciesReorderedCount = importCompetenciesResult.CompetencyReorderedCount;
            SkippedCount = importCompetenciesResult.SkippedCount;
            Errors = importCompetenciesResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
            FrameworkID = frameworkId;
            FrameworkName = frameworkName;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(frameworkVocabulary);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(frameworkVocabulary);
        }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ProcessedCount { get; set; }
        public int CompetenciesInsertedCount { get; set; }
        public int CompetenciesUpdatedCount { get; set; }
        public int CompetenciesReorderedCount { get; set; }
        public int CompetencyGroupsInsertedCount { get; set; }
        public int CompetencyGroupsUpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FrameworkID { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        private string MapReasonToErrorMessage(ImportCompetenciesResult.ErrorReason reason)
        {
            return reason switch
            {
                ImportCompetenciesResult.ErrorReason.MissingCompetencyName =>
                    "Competency name was not provided. Competency name is a required field and cannot be left blank",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyName => "Competency name must be 500 characters or fewer",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName => "Competency group name must be 255 characters or fewer",
                _ => "Unknown error",
            };
        }
    }
}
