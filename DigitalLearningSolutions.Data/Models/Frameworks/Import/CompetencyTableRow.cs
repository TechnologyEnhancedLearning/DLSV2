namespace DigitalLearningSolutions.Data.Models.Frameworks.Import
{
    using ClosedXML.Excel;
    using Org.BouncyCastle.Asn1.X509;

    public enum RowStatus
    {
        NotYetProcessed,
        Skipped,
        CompetencyGroupAndCompetencyInserted,
        CompetencyInserted,
        CompetencyUpdated,
        CompetencyGroupInserted,
        CompetencyGroupUpdated,
        CompetencyGroupAndCompetencyUpdated
    }
    public class CompetencyTableRow : BulkCompetency
    {
        public CompetencyTableRow(IXLTable table, IXLRangeRow row)
        {
            string? FindFieldValue(string name)
            {
                var colNumber = table.FindColumn(col => col.FirstCell().Value.ToString()?.ToLower() == name.ToLower()).ColumnNumber();
                return row.Cell(colNumber).GetValue<string?>();
            }

            RowNumber = row.RowNumber();
            id = row.Cell(1).GetValue<int?>();
            CompetencyGroup = FindFieldValue("CompetencyGroup");
            Competency = FindFieldValue("Competency");
            CompetencyDescription = FindFieldValue("CompetencyDescription");
            GroupDescription = FindFieldValue("GroupDescription");
            FlagsCsv = FindFieldValue("FlagsCSV");
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
