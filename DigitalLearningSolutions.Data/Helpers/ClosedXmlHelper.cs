namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.Data;
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

        public static void FormatWorksheetColumn(
            IXLWorkbook workbook,
            DataTable dataTable,
            string columnName,
            XLDataType dataType
        )
        {
            var columnNumber = dataTable.Columns.IndexOf(columnName) + 1;
            workbook.Worksheet(1).Column(columnNumber).CellsUsed(c => c.Address.RowNumber != 1)
                .SetDataType(dataType);
        }
    }
}
