namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseDelegatesDownloadFileService
    {
        public byte[] GetCourseDelegateDownloadFileForCourse(int customisationId, int centreId);
    }

    public class CourseDelegatesDownloadFileService : ICourseDelegatesDownloadFileService
    {
        private const string LastName = "Last name";
        private const string FirstName = "First name";
        private const string Email = "Email";
        private const string DelegateId = "Delegate ID";
        private const string Enrolled = "Enrolled";
        private const string LastAccessed = "Last accessed";
        private const string CompleteBy = "Complete by";
        private const string CompletedDate = "Completed date";
        private const string Logins = "Logins";
        private const string TimeMinutes = "Time (minutes)";
        private const string DiagnosticScore = "Diagnostic score";
        private const string AssessmentsPassed = "Assessments passed";
        private const string PassRate = "Pass rate";
        private const string Active = "Active";
        private const string RemovedDate = "Removed date";
        private const string Locked = "Locked";

        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;
        private readonly ICentreCustomPromptsService customPromptsService;

        public CourseDelegatesDownloadFileService(
            ICourseDelegatesDataService courseDelegatesDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            ICentreCustomPromptsService customPromptsService
        )
        {
            this.courseDelegatesDataService = courseDelegatesDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.customPromptsService = customPromptsService;
        }

        public byte[] GetCourseDelegateDownloadFileForCourse(int customisationId, int centreId)
        {
            using var workbook = new XLWorkbook();

            PopulateCourseDelegatesSheetForCourse(workbook, customisationId, centreId);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateCourseDelegatesSheetForCourse(
            IXLWorkbook workbook,
            int customisationId,
            int centreId
        )
        {
            var adminFields = courseAdminFieldsService.GetCustomPromptsForCourse(customisationId);

            var customRegistrationPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            var courseDelegates = courseDelegatesDataService.GetDelegatesOnCourseForExport(customisationId, centreId);

            var dataTable = new DataTable();

            SetUpDataTableColumns(customRegistrationPrompts, adminFields, dataTable);

            foreach (var courseDelegate in courseDelegates.OrderBy(x => x.LastName))
            {
                AddDelegateToDataTable(dataTable, courseDelegate, customRegistrationPrompts, adminFields);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                $"Course {customisationId}",
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatWorksheetColumns(workbook, dataTable);
        }

        private static void SetUpDataTableColumns(
            CentreCustomPrompts customRegistrationPrompts,
            CourseAdminFields adminFields,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[] { new DataColumn(LastName), new DataColumn(FirstName), new DataColumn(Email) }
            );

            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
            {
                dataTable.Columns.Add(
                    !dataTable.Columns.Contains(prompt.CustomPromptText)
                        ? prompt.CustomPromptText
                        : $"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"
                );
            }

            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(DelegateId), new DataColumn(Enrolled), new DataColumn(LastAccessed),
                    new DataColumn(CompleteBy), new DataColumn(CompletedDate), new DataColumn(Logins),
                    new DataColumn(TimeMinutes), new DataColumn(DiagnosticScore),
                    new DataColumn(AssessmentsPassed), new DataColumn(PassRate), new DataColumn(Active),
                    new DataColumn(RemovedDate), new DataColumn(Locked),
                }
            );

            foreach (var prompt in adminFields.AdminFields)
            {
                dataTable.Columns.Add(
                    !dataTable.Columns.Contains(prompt.CustomPromptText)
                        ? prompt.CustomPromptText
                        : $"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"
                );
            }
        }

        private static void AddDelegateToDataTable(
            DataTable dataTable,
            CourseDelegateForExport courseDelegate,
            CentreCustomPrompts customRegistrationPrompts,
            CourseAdminFields adminFields
        )
        {
            var row = dataTable.NewRow();

            row[LastName] = courseDelegate.LastName;
            row[FirstName] = courseDelegate.FirstName;
            row[Email] = courseDelegate.EmailAddress;

            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"))
                {
                    row[$"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"] =
                        courseDelegate.CustomRegistrationPromptAnswers[prompt.CustomPromptNumber - 1];
                }
                else
                {
                    row[prompt.CustomPromptText] =
                        courseDelegate.CustomRegistrationPromptAnswers[prompt.CustomPromptNumber - 1];
                }
            }

            row[DelegateId] = courseDelegate.CandidateNumber;
            row[Enrolled] = courseDelegate.Enrolled.Date;
            row[LastAccessed] = courseDelegate.LastUpdated.Date;
            row[CompleteBy] = courseDelegate.CompleteByDate?.Date;
            row[CompletedDate] = courseDelegate.Completed?.Date;
            row[Logins] = courseDelegate.LoginCount;
            row[TimeMinutes] = courseDelegate.Duration;
            row[DiagnosticScore] = courseDelegate.DiagnosticScore;
            row[AssessmentsPassed] = courseDelegate.AttemptsPassed;
            row[PassRate] = courseDelegate.PassRate;
            row[Active] = courseDelegate.Active;
            row[RemovedDate] = courseDelegate.RemovedDate?.Date;
            row[Locked] = courseDelegate.Locked;

            foreach (var prompt in adminFields.AdminFields)
            {
                if (dataTable.Columns.Contains($"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"))
                {
                    row[$"{prompt.CustomPromptText} (Prompt {prompt.CustomPromptNumber})"] =
                        courseDelegate.CustomAdminFieldAnswers[prompt.CustomPromptNumber - 1];
                }
                else
                {
                    row[prompt.CustomPromptText] =
                        courseDelegate.CustomAdminFieldAnswers[prompt.CustomPromptNumber - 1];
                }
            }

            dataTable.Rows.Add(row);
        }

        private static void FormatWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            var dateColumns = new[] { Enrolled, LastAccessed, CompleteBy, CompletedDate, RemovedDate };
            foreach (var columnName in dateColumns)
            {
                var columnIndex = dataTable.Columns.IndexOf(columnName) + 1;
                workbook.Worksheet(1).Column(columnIndex).CellsUsed(c => c.Address.RowNumber != 1)
                    .SetDataType(XLDataType.DateTime);
            }

            var numberColumns = new[] { Logins, TimeMinutes, DiagnosticScore, AssessmentsPassed, PassRate };
            foreach (var columnName in numberColumns)
            {
                var columnIndex = dataTable.Columns.IndexOf(columnName) + 1;
                workbook.Worksheet(1).Column(columnIndex).CellsUsed(c => c.Address.RowNumber != 1)
                    .SetDataType(XLDataType.Number);
            }

            var boolColumns = new[] { Active, Locked };
            foreach (var columnName in boolColumns)
            {
                var columnIndex = dataTable.Columns.IndexOf(columnName) + 1;
                workbook.Worksheet(1).Column(columnIndex).CellsUsed(c => c.Address.RowNumber != 1)
                    .SetDataType(XLDataType.Boolean);
            }
        }
    }
}
