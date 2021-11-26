namespace DigitalLearningSolutions.Data.Models.Frameworks.Import
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportCompetenciesResult
    {
        public enum ErrorReason
        {
            MissingCompetencyName,
            TooLongCompetencyGroupName,
            TooLongCompetencyName,
            AlreadyExists
        }
        public ImportCompetenciesResult() { }

        public ImportCompetenciesResult(
            IReadOnlyCollection<CompetencyTableRow> competencyTableRows
        )
        {
            ProcessedCount = competencyTableRows.Count;
            CompetenciesInsertedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.CompetencyInserted | dr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            CompetencyGroupsInsertedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.CompetencyGroupInserted | dr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            SkippedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.Skipped);
            Errors = competencyTableRows.Where(dr => dr.Error.HasValue).Select(dr => (dr.RowNumber, dr.Error!.Value));
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)> Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int CompetenciesInsertedCount { get; set; }
        public int CompetencyGroupsInsertedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
