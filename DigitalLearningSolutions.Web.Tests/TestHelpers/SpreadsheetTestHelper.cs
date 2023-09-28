namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Linq;
    using ClosedXML.Excel;
    using FluentAssertions;
    using FluentAssertions.Execution;

    public static class SpreadsheetTestHelper
    {
        public static void AssertSpreadsheetsAreEquivalent(XLWorkbook expectedWorkbook, XLWorkbook? resultWorkbook)
        {
            using (new AssertionScope())
            {
                resultWorkbook.Should().NotBeNull();

                resultWorkbook!.Worksheets.Count.Should().Be(expectedWorkbook.Worksheets.Count);
                foreach (var resultWorksheet in resultWorkbook.Worksheets)
                {
                    var expectedWorksheet = expectedWorkbook.Worksheets.Worksheet(resultWorksheet.Name);
                    var cells = resultWorksheet.CellsUsed();
                    cells.Count().Should().Be(expectedWorksheet.CellsUsed().Count());

                    foreach (var cell in cells)
                    {
                        var expectedCell = expectedWorksheet.Cell(cell.Address);
                        cell.Value.Should().BeEquivalentTo(expectedCell.Value);
                    }
                }
            }
        }
    }
}
