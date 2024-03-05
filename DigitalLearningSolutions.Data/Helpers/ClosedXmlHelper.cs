namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml.Spreadsheet;

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

        public static void HideWorkSheetColumn(
            IXLWorkbook workbook,
            string columnName,
            int workSheetNumber = 1
            )
        {
            foreach (var cell in workbook.Worksheet(workSheetNumber).Row(1).Cells())
            {
                if (cell.Value.ToString() == columnName)
                {
                    var columnNumber = cell.Address.ColumnNumber;
                    workbook.Worksheet(workSheetNumber).Column(columnNumber).Hide();
                    break;
                }
            }
        }

        public static void RenameWorksheetColumn(
            IXLWorkbook workbook,
            string columnName,
            string newName,
            int workSheetNumber = 1
            )
        {
            foreach (var cell in workbook.Worksheet(workSheetNumber).Row(1).Cells())
            {
                if(cell.Value.ToString() == columnName)
                {
                    cell.Value = newName;
                    break;
                }
            }
        }

        public static void AddValidationListToWorksheetColumn(
            IXLWorkbook workbook,
            int targetColumn,
            List<string> optionsList,
            int workSheetNumber = 1
            )
        {
            var listOptions = $"\"{String.Join(",", optionsList)}\"";
            workbook.Worksheet(workSheetNumber).Column(targetColumn).SetDataValidation().List(listOptions, true);
        }

        public static void AddValidationRangeToWorksheetColumn(
             IXLWorkbook workbook,
            int targetColumn,
            int targetWorkSheetNumber,
            int optionsCount,
            int sourceWorksheetNumber
            )
        {
            string sourceRange = "A2:A" + (optionsCount + 1).ToString();
            workbook.Worksheet(targetWorkSheetNumber).Column(targetColumn).SetDataValidation().List(workbook.Worksheet(sourceWorksheetNumber).Range(sourceRange), true);
        }

        public static void FormatWorksheetColumn(
            IXLWorkbook workbook,
            DataTable dataTable,
            string columnName,
            XLDataType dataType,
            int workSheetNumber = 1
        )
        {
            var columnNumber = dataTable.Columns.IndexOf(columnName) + 1;
            workbook.Worksheet(workSheetNumber).Column(columnNumber).CellsUsed(c => c.Address.RowNumber != 1)
                .SetDataType(dataType);
        }
    }
}
