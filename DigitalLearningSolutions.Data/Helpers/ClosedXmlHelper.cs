namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using ClosedXML.Excel;

    public static class ClosedXmlHelper
    {
        public static void AddSheetToWorkbook(
            IXLWorkbook workbook,
            string sheetName,
            IEnumerable<object>? dataObjects,
            XLTableTheme tableTheme
        )
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = tableTheme;
            sheet.Columns().AdjustToContents();
        }
    }
}
