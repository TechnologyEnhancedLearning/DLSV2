using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DigitalLearningSolutions.Data.Tests")]

namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Http;

    public interface IImportCompetenciesFromFileService
    {
        byte[] GetCompetencyFileForFramework(int frameworkId, bool v);
        public ImportCompetenciesResult PreProcessCompetenciesTable(IXLWorkbook workbook);
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId);
    }
    public class ImportCompetenciesFromFileService : IImportCompetenciesFromFileService
    {
        private readonly IFrameworkService frameworkService;
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        public const string CompetenciesSheetName = "CompetenciesBulkUpload";
        public ImportCompetenciesFromFileService(
           IFrameworkService frameworkService
       )
        {
            this.frameworkService = frameworkService;
        }
        public ImportCompetenciesResult PreProcessCompetenciesTable(IXLWorkbook workbook)
        {
            var table = OpenCompetenciesTable(workbook);
            var competencyRows = table.Rows().Skip(1).Select(row => new CompetencyTableRow(table, row)).ToList();
            foreach (var competencyRow in competencyRows)
            {
                PreProcessCompetencyRow(competencyRow);
            }
            return new ImportCompetenciesResult(competencyRows);
        }
        private void PreProcessCompetencyRow(CompetencyTableRow competencyRow)
        {
            if (competencyRow.id == null)
            {
                competencyRow.RowStatus = RowStatus.CompetencyInserted;
            }
            else
            {
                competencyRow.RowStatus = RowStatus.CompetencyUpdated;
            }
        }
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId)
        {
            int maxFrameworkCompetencyId = frameworkService.GetMaxFrameworkCompetencyID();
            int maxFrameworkCompetencyGroupId = frameworkService.GetMaxFrameworkCompetencyGroupID();
            var table = OpenCompetenciesTable(workbook);
            return ProcessCompetenciesTable(table, adminUserId, frameworkId, maxFrameworkCompetencyId, maxFrameworkCompetencyGroupId);
        }
        internal IXLTable OpenCompetenciesTable(IXLWorkbook workbook)
        {
            var worksheet = workbook.Worksheet(1);
            worksheet.Columns(1, 15).Unhide();
            if (worksheet.Tables.Count() == 0)
            {
                throw new InvalidHeadersException();
            }
            var table = worksheet.Tables.Table(0);
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }
            return table;
        }
        internal ImportCompetenciesResult ProcessCompetenciesTable(IXLTable table, int adminUserId, int frameworkId, int maxFrameworkCompetencyId, int maxFrameworkCompetencyGroupId)
        {
            var competenciesRows = table.Rows().Skip(1).Select(row => new CompetencyTableRow(table, row)).ToList();

            foreach (var competencyRow in competenciesRows)
            {
                maxFrameworkCompetencyGroupId = ProcessCompetencyRow(adminUserId, frameworkId, maxFrameworkCompetencyId, maxFrameworkCompetencyGroupId, competencyRow);
            }

            return new ImportCompetenciesResult(competenciesRows);
        }
        private int ProcessCompetencyRow(
            int adminUserId,
            int frameworkId,
            int maxFrameworkCompetencyId,
            int maxFrameworkCompetencyGroupId,
            CompetencyTableRow competencyRow
        )
        {
            if (!competencyRow.Validate())
            {
                return maxFrameworkCompetencyGroupId;
            }
            //If competency group is set, check if competency group exists within framework and add if not and get the Framework Competency Group ID
            int? frameworkCompetencyGroupId = null;
            if (competencyRow.CompetencyGroup != null)
            {
                var newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyRow.CompetencyGroup, null, adminUserId);
                if (newCompetencyGroupId > 0)
                {
                    frameworkCompetencyGroupId = frameworkService.InsertFrameworkCompetencyGroup(newCompetencyGroupId, frameworkId, adminUserId);
                    if (frameworkCompetencyGroupId > maxFrameworkCompetencyGroupId)
                    {
                        maxFrameworkCompetencyGroupId = (int)frameworkCompetencyGroupId;
                        competencyRow.RowStatus = RowStatus.CompetencyGroupInserted;
                    }
                }
            }

            //Check if competency already exists in framework competency group and add if not
            var newCompetencyId = frameworkService.InsertCompetency(competencyRow.Competency, competencyRow.CompetencyDescription, adminUserId);
            if (newCompetencyId > 0)
            {
                var newFrameworkCompetencyId = frameworkService.InsertFrameworkCompetency(newCompetencyId, frameworkCompetencyGroupId, adminUserId, frameworkId);
                if (newFrameworkCompetencyId > maxFrameworkCompetencyId)
                {
                    competencyRow.RowStatus = (competencyRow.RowStatus == RowStatus.CompetencyGroupInserted ? RowStatus.CompetencyGroupAndCompetencyInserted : RowStatus.CompetencyInserted);
                }
                else
                {
                    competencyRow.RowStatus = RowStatus.Skipped;
                }
            }
            return maxFrameworkCompetencyGroupId;
        }

        private static bool ValidateHeaders(IXLTable table)
        {
            var expectedHeaders = new List<string>
            {
                "ID",
                "CompetencyGroup",
                "GroupDescription",
                "Competency",
                "CompetencyDescription",
                "AlwaysShowDescription",
                "FlagsCSV"
            }.OrderBy(x => x);
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }

        public byte[] GetCompetencyFileForFramework(int frameworkId, bool blank)
        {
            using var workbook = new XLWorkbook();
            PopulateCompetenciesSheet(workbook, frameworkId, blank);
            if (blank)
            {
                ClosedXmlHelper.HideWorkSheetColumn(workbook, "ID");
            }
            var options = new List<string> { "TRUE", "FALSE" };
            ClosedXmlHelper.AddValidationListToWorksheetColumn(workbook, 6, options);
            var rowCount = workbook.Worksheet(1).RangeUsed().RowCount();
            ClosedXmlHelper.AddValidationRangeToWorksheetColumn(workbook, 1, 1, rowCount, 1);
            // Calculate the workbook
            workbook.CalculateMode = XLCalculateMode.Auto;
            workbook.RecalculateAllFormulas();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        private void PopulateCompetenciesSheet(IXLWorkbook workbook, int frameworkId, bool blank)
        {


            var competencyRecords = frameworkService.GetBulkCompetenciesForFramework(blank ? 0 : frameworkId);
            var competencies = competencyRecords.Select(
                x => new
                {
                    ID = x.id,
                    x.CompetencyGroup,
                    x.GroupDescription,
                    x.Competency,
                    x.CompetencyDescription,
                    x.AlwaysShowDescription,
                    FlagsCSV = x.FlagsCsv,
                }
            );

            ClosedXmlHelper.AddSheetToWorkbook(workbook, CompetenciesSheetName, competencies, TableTheme);
        }
    }
}
