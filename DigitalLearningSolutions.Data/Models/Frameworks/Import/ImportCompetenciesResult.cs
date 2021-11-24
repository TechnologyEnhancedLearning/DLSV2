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
            RegisteredCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.Registered);
            UpdatedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.Updated);
            SkippedCount = competencyTableRows.Count(dr => dr.RowStatus == RowStatus.Skipped);
            Errors = competencyTableRows.Where(dr => dr.Error.HasValue).Select(dr => (dr.RowNumber, dr.Error!.Value));
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)> Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int RegisteredCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
