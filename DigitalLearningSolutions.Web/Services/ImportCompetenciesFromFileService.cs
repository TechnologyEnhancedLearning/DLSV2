﻿using System.Runtime.CompilerServices;

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
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using DocumentFormat.OpenXml.Office2010.Excel;

    public interface IImportCompetenciesFromFileService
    {
        byte[] GetCompetencyFileForFramework(int frameworkId, bool isBlank, string vocabulary);
        public ImportCompetenciesResult PreProcessCompetenciesTable(IXLWorkbook workbook, string vocabulary, int frameworkId);
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId, string vocabulary, int reorderCompetenciesOption, int addAssessmentQuestionsOption, int customAssessmentQuestionID, List<int> defaultQuestionIds);
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId, string vocabulary, int reorderCompetenciesOption, int addAssessmentQuestionsOption, int customAssessmentQuestionID, List<int> defaultQuestionIds);
    }
    public class ImportCompetenciesFromFileService : IImportCompetenciesFromFileService
    {
        private readonly IFrameworkService frameworkService;
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        public const string CompetenciesSheetName = "FrameworkBulkUpload";
        public ImportCompetenciesFromFileService(
           IFrameworkService frameworkService
       )
        {
            this.frameworkService = frameworkService;
        }
        public ImportCompetenciesResult PreProcessCompetenciesTable(IXLWorkbook workbook, string vocabulary, int frameworkId)
        {
            var table = OpenCompetenciesTable(workbook, vocabulary);
            var competencyRows = table.Rows().Skip(1).Select(row => new CompetencyTableRow(table, row)).ToList();
            var newCompetencyIds = competencyRows.Select(row => row.ID ?? 0).ToList();
            var existingIds = frameworkService.GetFrameworkCompetencyOrder(frameworkId, newCompetencyIds);
            foreach (var competencyRow in competencyRows)
            {
                PreProcessCompetencyRow(competencyRow, newCompetencyIds, existingIds);
            }
            return new ImportCompetenciesResult(competencyRows);
        }
        private void PreProcessCompetencyRow(CompetencyTableRow competencyRow, List<int> newIds, List<int> existingIds)
        {
            if (competencyRow.ID == null)
            {
                competencyRow.RowStatus = RowStatus.CompetencyInserted;
            }
            else
            {
                var id = (int)(competencyRow?.ID);
                if (!existingIds.Contains(id))
                {
                    competencyRow.RowStatus = RowStatus.InvalidId;
                }
                else
                {
                    int originalIndex = existingIds.IndexOf(id);
                    int newIndex = newIds.IndexOf(id);
                    competencyRow.RowStatus = RowStatus.CompetencyUpdated;
                    if (originalIndex != newIndex)
                    {
                        competencyRow.Reordered = true;
                    }
                }
            }
            competencyRow.Validate();
        }
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId, string vocabulary, int reorderCompetenciesOption, int addAssessmentQuestionsOption, int customAssessmentQuestionID, List<int> defaultQuestionIds)
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IXLWorkbook workbook, int adminUserId, int frameworkId, string vocabulary, int reorderCompetenciesOption, int addAssessmentQuestionsOption, int customAssessmentQuestionID, List<int> defaultQuestionIds)
        {
            int maxFrameworkCompetencyId = frameworkService.GetMaxFrameworkCompetencyID();
            int maxFrameworkCompetencyGroupId = frameworkService.GetMaxFrameworkCompetencyGroupID();
            var table = OpenCompetenciesTable(workbook, vocabulary);
            return ProcessCompetenciesTable(table, adminUserId, frameworkId, maxFrameworkCompetencyId, maxFrameworkCompetencyGroupId, addAssessmentQuestionsOption, customAssessmentQuestionID, defaultQuestionIds);
        }
        internal IXLTable OpenCompetenciesTable(IXLWorkbook workbook, string vocabulary)
        {
            var worksheet = workbook.Worksheet(1);
            worksheet.Columns(1, 15).Unhide();
            if (worksheet.Tables.Count() == 0)
            {
                throw new InvalidHeadersException();
            }
            var table = worksheet.Tables.Table(0);
            if (!ValidateHeaders(table, vocabulary))
            {
                throw new InvalidHeadersException();
            }
            return table;
        }
        internal ImportCompetenciesResult ProcessCompetenciesTable(IXLTable table, int adminUserId, int frameworkId, int maxFrameworkCompetencyId, int maxFrameworkCompetencyGroupId, int addAssessmentQuestionsOption, int customAssessmentQuestionID, List<int> defaultQuestionIds)
        {
            var competenciesRows = table.Rows().Skip(1).Select(row => new CompetencyTableRow(table, row)).ToList();
            int rowCount = 0;
            string currentGroup = null;
            competenciesRows = competenciesRows
            .Select(row =>
            {
                if (row.CompetencyGroup != currentGroup)
                {
                    currentGroup = row.CompetencyGroup;
                    rowCount = 1;
                }
                else
                {
                    rowCount++;
                }
                row.CompetencyOrderNumber = rowCount;
                return row;
            })
            .ToList();
            var competencyGroupCount = competenciesRows
            .Where(row => !string.IsNullOrWhiteSpace(row.CompetencyGroup))
            .Select(row => row.CompetencyGroup)
            .Distinct()
            .Count();
            foreach (var competencyRow in competenciesRows)
            {
                maxFrameworkCompetencyGroupId = ProcessCompetencyRow(adminUserId, frameworkId, maxFrameworkCompetencyId, maxFrameworkCompetencyGroupId, addAssessmentQuestionsOption, customAssessmentQuestionID, defaultQuestionIds, competencyRow);
            }
            // Check for changes to competency group order and apply them if appropriate:
            if (reorderCompetenciesOption == 2)
            {
                var distinctCompetencyGroups = competenciesRows
                    .Where(row => !string.IsNullOrWhiteSpace(row.CompetencyGroup))
                    .Select(row => row.CompetencyGroup)
                    .Distinct()
                    .ToList();
                for (int i = 0; i < competencyGroupCount; i++)
                {
                    var existingGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId).Select(row => new { row.ID, row.Name })
                   .Distinct()
                   .ToList();
                    var placesToMove = Math.Abs(existingGroups.FindIndex(group => group.Name == distinctCompetencyGroups[i])-i);
                    if (placesToMove > 0)
                    {
                        var thisGroup = existingGroups.FirstOrDefault(group => group.Name == distinctCompetencyGroups[i]);
                        var direction = existingGroups.FindIndex(group => group.Name == distinctCompetencyGroups[i]) > i ? "UP" : "DOWN";
                        for (int p = 0; p < placesToMove; p++)
                        {
                            frameworkService.MoveFrameworkCompetencyGroup(thisGroup.ID, true, direction);
                        }
                        competenciesRows
                                .Where(row => row.CompetencyGroup == thisGroup.Name)
                                .ToList()
                                .ForEach(row => row.Reordered = true);
                    }
                }
            }
            return new ImportCompetenciesResult(competenciesRows);
        }
        private int ProcessCompetencyRow(
            int adminUserId,
            int frameworkId,
            int maxFrameworkCompetencyId,
            int maxFrameworkCompetencyGroupId,
            int addAssessmentQuestionsOption,
            int customAssessmentQuestionID,
            List<int> defaultQuestionIds,
            CompetencyTableRow competencyRow
        )
        {
            if (!competencyRow.Validate())
            {
                return maxFrameworkCompetencyGroupId;
            }
            int newCompetencyGroupId = 0;
            int newCompetencyId = 0;
            //If competency group is set, check if competency group exists within framework and add if not and get the Framework Competency Group ID
            int ? frameworkCompetencyGroupId = null;
            if (competencyRow.CompetencyGroup != null)
            {
                newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyRow.CompetencyGroup, competencyRow.GroupDescription, adminUserId);
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
            // If FrameworkCompetency ID is supplied, update the competency
            if (competencyRow.ID != null)
            {
                var frameworkCompetency = frameworkService.GetFrameworkCompetencyById((int)competencyRow.ID);
                if (frameworkCompetency != null)
                {
                    newCompetencyId = frameworkCompetency.CompetencyID;
                    if (frameworkCompetency.Name != competencyRow.Competency || frameworkCompetency.Description != competencyRow.CompetencyDescription || frameworkCompetency.AlwaysShowDescription != competencyRow.AlwaysShowDescription )
                    {
                        frameworkService.UpdateFrameworkCompetency((int)competencyRow.ID, competencyRow.Competency, competencyRow.CompetencyDescription, adminUserId, competencyRow.AlwaysShowDescription ?? false);
                        competencyRow.RowStatus = (competencyRow.RowStatus == RowStatus.CompetencyGroupInserted ? RowStatus.CompetencyGroupAndCompetencyUpdated: RowStatus.CompetencyUpdated);
                    }
                    else
                    {
                        competencyRow.RowStatus = RowStatus.Skipped;
                    }
                }
            }
            else
            {
                //Check if competency already exists in framework competency group and add if not
                newCompetencyId = frameworkService.InsertCompetency(competencyRow.Competency, competencyRow.CompetencyDescription, adminUserId);
                if (newCompetencyId > 0)
                {
                    var newFrameworkCompetencyId = frameworkService.InsertFrameworkCompetency(newCompetencyId, frameworkCompetencyGroupId, adminUserId, frameworkId, competencyRow.AlwaysShowDescription ?? false); //including always show desc flag
                    if (newFrameworkCompetencyId > maxFrameworkCompetencyId)
                    {
                        competencyRow.RowStatus = (competencyRow.RowStatus == RowStatus.CompetencyGroupInserted ? RowStatus.CompetencyGroupAndCompetencyInserted : RowStatus.CompetencyInserted);
                    }
                    else
                    {
                        competencyRow.RowStatus = RowStatus.Skipped;
                    }
                }
            }


            // If flags are supplied, add them:
            if (competencyRow.FlagsCsv != null)
            {
                var flags = competencyRow.FlagsCsv.Split(',');
                int[] flagIds = [];
                foreach (var flag in flags) {
                    int flagId = 0;
                    var frameworkFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, null, null);
                    if (frameworkFlags.Any())
                    {
                        foreach (var frameworkFlag in frameworkFlags)
                        {
                            if (frameworkFlag.FlagName == flag)
                            {
                                flagId = frameworkFlag.FlagId;
                            }
                        }
                    }
                    if (flagId == 0)
                    {
                        flagId = frameworkService.AddCustomFlagToFramework(frameworkId, flag, "Flag", "nhsuk-tag--white");
                    }
                    flagIds.Append(flagId);
                }
                if (flagIds.Any()) {
                    frameworkService.UpdateCompetencyFlags(frameworkId, newCompetencyId, flagIds);
                }
            }


            // Add assessment questions if necessary:
            if (defaultQuestionIds.Count > 0 | customAssessmentQuestionID > 0)
            {
                if (competencyRow.RowStatus == RowStatus.CompetencyInserted | competencyRow.RowStatus == RowStatus.CompetencyGroupAndCompetencyInserted || addAssessmentQuestionsOption == 2 && competencyRow.RowStatus == RowStatus.CompetencyUpdated | competencyRow.RowStatus == RowStatus.CompetencyGroupAndCompetencyUpdated || addAssessmentQuestionsOption == 3)
                {
                    foreach(var id in defaultQuestionIds)
                    {
                        frameworkService.AddCompetencyAssessmentQuestion((int)competencyRow.ID, id, adminUserId);
                    }
                    if(customAssessmentQuestionID > 0)
                    {
                        frameworkService.AddCompetencyAssessmentQuestion((int)competencyRow.ID, customAssessmentQuestionID, adminUserId);
                    }
                }
            }

            return maxFrameworkCompetencyGroupId;
        }

        private static bool ValidateHeaders(IXLTable table, string Vocabulary)
        {
            var expectedHeaders = new List<string>
            {
                "ID",
                Vocabulary + "Group",
                "GroupDescription",
                Vocabulary,
                Vocabulary + "Description",
                "AlwaysShowDescription",
                "FlagsCSV"
            }.OrderBy(x => x);
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }

        public byte[] GetCompetencyFileForFramework(int frameworkId, bool blank, string vocabulary)
        {
            using var workbook = new XLWorkbook();
            PopulateCompetenciesSheet(workbook, frameworkId, blank, vocabulary);
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
        private void PopulateCompetenciesSheet(IXLWorkbook workbook, int frameworkId, bool blank, string vocabulary)
        {
            var competencyRecords = frameworkService.GetBulkCompetenciesForFramework(blank ? 0 : frameworkId);
            var competencies = competencyRecords.Select(
                x => new
                {
                    x.ID,
                    x.CompetencyGroup,
                    x.GroupDescription,
                    x.Competency,
                    x.CompetencyDescription,
                    x.AlwaysShowDescription,
                    FlagsCSV = x.FlagsCsv,
                }
            );
            ClosedXmlHelper.AddSheetToWorkbook(workbook, CompetenciesSheetName, competencies, TableTheme);
            ClosedXmlHelper.RenameWorksheetColumn(workbook, "CompetencyGroup", vocabulary + "Group");
            ClosedXmlHelper.RenameWorksheetColumn(workbook, "Competency", vocabulary);
            ClosedXmlHelper.RenameWorksheetColumn(workbook, "CompetencyDescription", vocabulary + "Description");
        }
    }
}
