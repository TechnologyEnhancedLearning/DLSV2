using System.Collections.Generic;
using System;
using DigitalLearningSolutions.Web.Models;
using System.Linq;
using DigitalLearningSolutions.Data.Models.Frameworks.Import;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class ImportCompetenciesPreProcessViewModel
    {
        public ImportCompetenciesPreProcessViewModel(ImportCompetenciesResult bulkCompetenciesResult, BulkCompetenciesData bulkCompetenciesData)
        {
            FrameworkName = bulkCompetenciesData.FrameworkName;
            PublishStatusID = bulkCompetenciesData.PublishStatusID;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(bulkCompetenciesData.FrameworkVocubulary);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(bulkCompetenciesData.FrameworkVocubulary);
            ToProcessCount = bulkCompetenciesResult.ProcessedCount;
            CompetenciesToAddCount = bulkCompetenciesResult.CompetencyAddedCount;
            CompetenciesToReorderCount = bulkCompetenciesResult.CompetencyReorderedCount;
            ToUpdateOrSkipCount = bulkCompetenciesResult.CompetencyUpdatedCount;
            Errors = bulkCompetenciesResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason, FrameworkVocabularyHelper.VocabularySingular(bulkCompetenciesData.FrameworkVocubulary))));
            FlagCount = bulkCompetenciesResult.FlagCount;
            DistinctFlagsCount = bulkCompetenciesResult.DistinctFlagsCount;
            CompetencyGroupCount = bulkCompetenciesResult.CompetencyGroupCount;
        }
        public string? FrameworkName { get; set; }
        public int PublishStatusID { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToReorderCount { get; set; }
        public int ToUpdateOrSkipCount { get; set; }
        public string? ImportFile { get; set; }
        public bool IsNotBlank { get; set; }
        public string TabName { get; set; }
        public int FlagCount { get; set; }
        public int DistinctFlagsCount { get; set; }
        public int CompetencyGroupCount { get; set; }

        private static string MapReasonToErrorMessage(ImportCompetenciesResult.ErrorReason reason, string vocabularySingular)
        {
            return reason switch
            {
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName =>
                    "Group name must be 255 characters or less.",
                ImportCompetenciesResult.ErrorReason.MissingCompetencyName =>
                    vocabularySingular + " is blank. " + vocabularySingular + " is a required field and cannot be left blank",
                ImportCompetenciesResult.ErrorReason.InvalidId =>
                    "The ID provided does not match a " + vocabularySingular + " ID in this Framework. Leave the ID column blank for new competencies.",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyName =>
                    vocabularySingular + " must be 500 characters or less.",
                ImportCompetenciesResult.ErrorReason.InvalidAlwaysShowDescription =>
                    "Always show description is invalid. The Always show description field must contain 'TRUE' or 'FALSE'",
                _ => "Unspecified error.",
            };
        }
    }
}
