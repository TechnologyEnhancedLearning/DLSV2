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
    public class CompetencyTableRow
    {
        public CompetencyTableRow(IXLTable table, IXLRangeRow row)
        {
            string? FindFieldValue(string name)
            {
                var colNumber = table.FindColumn(col => col.FirstCell().Value.ToString().ToLower() == name).ColumnNumber();
                return row.Cell(colNumber).GetValue<string?>();
            }

            RowNumber = row.RowNumber();
            CompetencyGroupName = FindFieldValue("competency group");
            CompetencyName = FindFieldValue("competency name");
            CompetencyDescription = FindFieldValue("competency description");
            RowStatus = RowStatus.NotYetProcessed;
        }
        public int RowNumber { get; set; }
        public string? CompetencyGroupName { get; set; }
        public string? CompetencyName { get; set; }
        public string? CompetencyDescription { get; set; }
        public ImportCompetenciesResult.ErrorReason? Error { get; set; }
        public RowStatus RowStatus { get; set; }
        public bool Validate()
        {
            if (string.IsNullOrEmpty(CompetencyName))
            {
                Error = ImportCompetenciesResult.ErrorReason.MissingCompetencyName;
            }
            else if (CompetencyGroupName.Length > 255)
            {
                Error = ImportCompetenciesResult.ErrorReason.TooLongCompetencyGroupName;
            }
            else if (CompetencyName.Length > 500)
            {
                Error = ImportCompetenciesResult.ErrorReason.TooLongCompetencyName;
            }

            return !Error.HasValue;
        }
    }
}
