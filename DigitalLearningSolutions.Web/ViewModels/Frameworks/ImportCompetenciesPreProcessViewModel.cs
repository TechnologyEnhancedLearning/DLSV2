using System.Collections.Generic;
using System;
using DigitalLearningSolutions.Web.Models;
using System.Linq;
using DigitalLearningSolutions.Data.Models.Frameworks.Import;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class ImportCompetenciesPreProcessViewModel
    {
        public ImportCompetenciesPreProcessViewModel(ImportCompetenciesResult bulkCompetenciesResult)
        {
            ToProcessCount = bulkCompetenciesResult.ProcessedCount;
            CompetenciesToAddCount = bulkCompetenciesResult.CompetencyAddedCount;
            CompetenciesToUpdateCount = bulkCompetenciesResult.CompetencyUpdatedCount;
            CompetencyGroupsToAddCount = bulkCompetenciesResult.GroupAddedCount;
            CompetencyGroupsToUpdateCount = bulkCompetenciesResult.GroupUpdatedCount;
            Errors = bulkCompetenciesResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
        }

        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToUpdateCount { get; set; }
        public int CompetencyGroupsToAddCount { get; set; }
        public int CompetencyGroupsToUpdateCount { get; set; }
        public int ToUpdateOrSkipCount { get; set; }
        public string? ImportFile { get; set; }
        public bool IsNotBlank { get; set; }
        public string TabName { get; set; }

        private static string MapReasonToErrorMessage(ImportCompetenciesResult.ErrorReason reason)
        {
            return reason switch
            {
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName =>
                    "Group name must be 255 characters or less.",
                ImportCompetenciesResult.ErrorReason.MissingCompetencyName =>
                    "Competency is blank. Competency is a required field and cannot be left blank",
                ImportCompetenciesResult.ErrorReason.InvalidId =>
                    "The ID provided does not match a Competency ID in this Framework",
                ImportCompetenciesResult.ErrorReason.TooLongCompetencyName =>
                    "Competency must be 255 characters or less.",
                ImportCompetenciesResult.ErrorReason.InvalidAlwaysShowDescription =>
                    "Always show description is invalid. The Always show description field must contain 'TRUE' or 'FALSE'",
                _ => "Unspecified error.",
            };
        }
    }
}
