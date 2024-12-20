using DigitalLearningSolutions.Data.Models.Frameworks.Import;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Models
{
    public class BulkCompetenciesResult
    {
        public enum ErrorReason
        {
            InvalidId,
            MissingCompetencyName,
            TooLongCompetencyName,
            TooLongCompetencyGroupName,
            InvalidAlwaysShowDescription
        }

        public BulkCompetenciesResult() { }

        public BulkCompetenciesResult(
            IReadOnlyCollection<CompetencyTableRow> competencyRows
        )
        {
            ProcessedCount = competencyRows.Count;
            CompetencyAddedCount = competencyRows.Count(cr => cr.RowStatus == RowStatus.CompetencyInserted || cr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            GroupAddedCount = competencyRows.Count(cr => cr.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted);
            GroupUpdatedCount = competencyRows.Count(cr => cr.RowStatus == RowStatus.CompetencyGroupUpdated || cr.RowStatus == RowStatus.CompetencyGroupAndCompetencyUpdated);
            SkippedCount = competencyRows.Count(cr => cr.RowStatus == RowStatus.Skipped);
            Errors = (IEnumerable<(int RowNumber, ErrorReason Reason)>)competencyRows.Where(cr => cr.Error.HasValue).Select(cr => (cr.RowNumber, cr.Error!.Value));
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)>? Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int CompetencyAddedCount { get; set; }
        public int CompetencyUpdatedCount { get; set; }
        public int GroupAddedCount { get; set; }
        public int GroupUpdatedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
