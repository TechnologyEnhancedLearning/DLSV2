namespace DigitalLearningSolutions.Web.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.User;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;
    public interface IPlatformUsageSummaryDownloadFileService
    {
        public byte[] GetPlatformUsageSummaryFile();
    }
    public class PlatformUsageSummaryDownloadFileService: IPlatformUsageSummaryDownloadFileService
    {
        public const string PlatformUsageSheetName = "Platform Usage";

        private const string ActiveCentres = "Active centres";
        private const string learners = "Active learners";
        private const string LearnerLogins = "Learner logins";
        private const string CourseLearningTime = "Course learning time (hours)";

        private const string CourseEnrolments = "Course enrolments";
        private const string CourseCompletions = "Course completions";
        private const string IndependentSelfAssessmentEnrolments = "Independent self assessment enrolments";
        private const string IndependentSelfAssessmentCompletions = "Independent self assessment completions";
        private const string SupervisedSelfAssessmentEnrolments = "Supervised self assessment enrolments";
        private const string SupervisedSelfAssessmentCompletions = "Supervised self assessment completions";
        private readonly IPlatformReportsService platformReportsService;
        public PlatformUsageSummaryDownloadFileService(
            IPlatformReportsService platformReportsService
        )
        {
            this.platformReportsService = platformReportsService;
        }

        public byte[] GetPlatformUsageSummaryFile()
        {
            using var workbook = new XLWorkbook();

            PopulatePlatformUsageSummarySheet(workbook);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulatePlatformUsageSummarySheet(IXLWorkbook workbook)
        {
           var platformUsageSummaryToExport = platformReportsService.GetPlatformUsageSummary(); ;
            var dataTable = new DataTable();
            SetUpDataTableColumnsForAllAdmins(dataTable);
           SetPlatformUsageSummaryRowValues(dataTable, platformUsageSummaryToExport);
          
            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                PlatformUsageSheetName,
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatAllDelegateWorksheetColumns(workbook, dataTable);
        }
        private static void SetUpDataTableColumnsForAllAdmins(
            DataTable dataTable
        )
        {

            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(ActiveCentres),
                    new DataColumn(learners),
                    new DataColumn(LearnerLogins),

                    new DataColumn(CourseLearningTime),
                    new DataColumn(CourseEnrolments),
                    new DataColumn(CourseCompletions),
                    new DataColumn(IndependentSelfAssessmentEnrolments),

                    new DataColumn(IndependentSelfAssessmentCompletions),
                    new DataColumn(SupervisedSelfAssessmentEnrolments),
                    new DataColumn(SupervisedSelfAssessmentCompletions),

                  
                }
            );
        }

        private static void SetPlatformUsageSummaryRowValues(
            DataTable dataTable,
            PlatformUsageSummary platformUsageSummary
        )
        {
            var row = dataTable.NewRow();

            row[ActiveCentres] = platformUsageSummary.ActiveCentres;
            row[learners] = platformUsageSummary.Learners;
            row[LearnerLogins] = platformUsageSummary.LearnerLogins;
            row[CourseLearningTime] = platformUsageSummary.CourseLearningTime;
            row[CourseEnrolments] = platformUsageSummary.CourseEnrolments;
            row[CourseCompletions] = platformUsageSummary.CourseCompletions;

            
            row[IndependentSelfAssessmentEnrolments] = platformUsageSummary.IndependentSelfAssessmentEnrolments;
            row[IndependentSelfAssessmentCompletions] = platformUsageSummary.IndependentSelfAssessmentCompletions;
            row[SupervisedSelfAssessmentEnrolments] = platformUsageSummary.SupervisedSelfAssessmentEnrolments;

            row[SupervisedSelfAssessmentCompletions] = platformUsageSummary.SupervisedSelfAssessmentCompletions;
            dataTable.Rows.Add(row);
        }

        private static void FormatAllDelegateWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            var integerColumns = new[] { ActiveCentres, learners, LearnerLogins, CourseLearningTime, CourseEnrolments, CourseCompletions,
                                IndependentSelfAssessmentEnrolments, IndependentSelfAssessmentCompletions, SupervisedSelfAssessmentEnrolments,SupervisedSelfAssessmentCompletions };
            foreach (var columnName in integerColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Number);
            }
           
        }
    }
}
