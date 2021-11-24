﻿namespace DigitalLearningSolutions.Data.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Exceptions;

    public interface IImportCompetenciesFromFileService
    {
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IFormFile file, int adminUserId, int frameworkId);
    }
    public class ImportCompetenciesFromFileService : IImportCompetenciesFromFileService
    {
        private readonly IFrameworkService frameworkService;
        public ImportCompetenciesFromFileService(
           IFrameworkService frameworkService
       )
        {
            this.frameworkService = frameworkService;
        }
        public ImportCompetenciesResult ProcessCompetenciesFromFile(IFormFile file, int adminUserId, int frameworkId)
        {
            var table = OpenCompetenciesTable(file);
            return ProcessCompetenciesTable(table, adminUserId, frameworkId);
        }
        internal IXLTable OpenCompetenciesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(0);
            var table = worksheet.Tables.Table(0);
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }
            return table;
        }
        internal ImportCompetenciesResult ProcessCompetenciesTable(IXLTable table, int adminUserId, int frameworkId)
        {
            var competenciesRows = table.Rows().Skip(1).Select(row => new CompetencyTableRow(table, row)).ToList();

            foreach (var competencyRow in competenciesRows)
            {
                ProcessCompetencyRow(adminUserId, frameworkId, competencyRow);
            }

            return new ImportCompetenciesResult(competenciesRows);
        }
        private void ProcessCompetencyRow(
            int adminUserId,
            int frameworkId,
            CompetencyTableRow competencyRow
        )
        {
            if (!competencyRow.Validate())
            {
                return;
            }
            //If competency group is set, check if competency group exists within framework and add if not and get the Framework Competency Group ID
            int? frameworkCompetencyGroupId = null;
            if(competencyRow.CompetencyGroupName != null)
            {
                var newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyRow.CompetencyGroupName, adminUserId);
                if (newCompetencyGroupId > 0)
                {
                    frameworkCompetencyGroupId = frameworkService.InsertFrameworkCompetencyGroup(newCompetencyGroupId, frameworkId, adminUserId);
                    competencyRow.RowStatus = RowStatus.CompetencyGroupInserted;
                }
            }

            //Check if competency already exists in framework competency group and add if not
            var newCompetencyId = frameworkService.InsertCompetency(competencyRow.CompetencyName, competencyRow.CompetencyDescription, adminUserId);
            if (newCompetencyId > 0)
            {
                var newFrameworkCompetencyId = frameworkService.InsertFrameworkCompetency(newCompetencyId, frameworkCompetencyGroupId, adminUserId, frameworkId);
                if (newFrameworkCompetencyId > 0)
                {
                    competencyRow.RowStatus = (competencyRow.RowStatus == RowStatus.CompetencyGroupInserted ? RowStatus.CompetencyGroupAndCompetencyInserted : RowStatus.CompetencyInserted);
                }
                else
                {
                    competencyRow.RowStatus = RowStatus.Skipped;
                }
            }
        }

            private static bool ValidateHeaders(IXLTable table)
        {
            var expectedHeaders = new List<string>
            {
                "CompetencyGroupName",
                "CompetencyName",
                "CompetencyDescription"
            }.OrderBy(x => x);
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }
    }
}
