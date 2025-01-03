namespace DigitalLearningSolutions.Data.Models.Frameworks.Import
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportCompetenciesResult
    {
        public enum ErrorReason
        {
            InvalidId,
            MissingCompetencyName,
            TooLongCompetencyName,
            TooLongCompetencyGroupName,
            InvalidAlwaysShowDescription
        }
        public ImportCompetenciesResult() { }

        public ImportCompetenciesResult(
            IReadOnlyCollection<CompetencyTableRow> competencyTableRows
        )
        {
            ProcessedCount = competencyTableRows.Count;
            CompetencyAddedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.CompetencyInserted | dr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            CompetencyUpdatedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.CompetencyUpdated);
            GroupAddedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.CompetencyGroupInserted | dr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            SkippedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.Skipped);
            Errors = competencyTableRows.Where(dr => dr.Error.HasValue).Select(dr => (dr.RowNumber, dr.Error!.Value));
            FlagCount = competencyTableRows
                .Where(row => !string.IsNullOrWhiteSpace(row.FlagsCsv))
                .SelectMany(static row => row.FlagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Count();
            DistinctFlagsCount = competencyTableRows
                .Where(row => !string.IsNullOrWhiteSpace(row.FlagsCsv))
                .SelectMany(row => row.FlagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(flag => flag.Trim())
                .Distinct()
                .Count();
            CompetencyGroupCount = competencyTableRows
                .Where(row => !string.IsNullOrWhiteSpace(row.CompetencyGroup))
                .Select(static row => row.CompetencyGroup)
                .Distinct()
                .Count();
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)>? Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int CompetencyAddedCount { get; set; }
        public int CompetencyUpdatedCount { get; set; }
        public int GroupAddedCount { get; set; }
        public int GroupUpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FlagCount { get; set; }
        public int DistinctFlagsCount { get; set; }
        public int CompetencyGroupCount { get; set; }
    }
}
