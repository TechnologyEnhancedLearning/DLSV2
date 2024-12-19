namespace DigitalLearningSolutions.Data.Models.Frameworks.Import
{
    using ClosedXML.Excel;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public enum RowStatus
    {
        NotYetProcessed,
        Skipped,
        CompetencyGroupInserted,
        CompetencyGroupAndCompetencyInserted,
        CompetencyInserted
    }
    public class CompetencyTableRow : BulkCompetency
    {
        public CompetencyTableRow(IXLTable table, IXLRangeRow row)
        {
            string? FindFieldValue(string name)
            {
                var colNumber = table.FindColumn(col => col.FirstCell().Value.ToString()?.ToLower() == name).ColumnNumber();
                return row.Cell(colNumber).GetValue<string?>();
            }

            RowNumber = row.RowNumber();
            CompetencyGroup = FindFieldValue("competency group");
            Competency = FindFieldValue("competency name");
            CompetencyDescription = FindFieldValue("competency description");
            RowStatus = RowStatus.NotYetProcessed;
        }
        public int RowNumber { get; set; }
        public ImportCompetenciesResult.ErrorReason? Error { get; set; }
        public RowStatus RowStatus { get; set; }
        public bool Validate()
        {
            if (string.IsNullOrEmpty(Competency))
            {
                Error = ImportCompetenciesResult.ErrorReason.MissingCompetencyName;
            }
            else if (CompetencyGroup?.Length > 255)
            {
                Error = ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName;
            }
            else if (Competency.Length > 500)
            {
                Error = ImportCompetenciesResult.ErrorReason.TooLongCompetencyName;
            }

            return !Error.HasValue;
        }
    }
}
