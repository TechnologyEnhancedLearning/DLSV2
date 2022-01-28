namespace DigitalLearningSolutions.Data.Services
{
    using System;
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
        public byte[] GetCourseDelegateDownloadFileForCourse(int customisationId, int centreId, int? adminCategoryId);
    }

    public class CourseDelegatesDownloadFileService : ICourseDelegatesDownloadFileService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;
        private readonly ICourseService courseService;
        private readonly ICentreCustomPromptsService customPromptsService;

        public CourseDelegatesDownloadFileService(
            ICourseService courseService,
            ICourseDelegatesDataService courseDelegatesDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            ICentreCustomPromptsService customPromptsService
        )
        {
            this.courseService = courseService;
            this.courseDelegatesDataService = courseDelegatesDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.customPromptsService = customPromptsService;
        }

        public byte[] GetCourseDelegateDownloadFileForCourse(int customisationId, int centreId, int? adminCategoryId)
        {
            using var workbook = new XLWorkbook();

            PopulateCourseDelegatesSheetForCourse(workbook, customisationId, centreId, adminCategoryId);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateCourseDelegatesSheetForCourse(
            IXLWorkbook workbook,
            int customisationId,
            int centreId,
            int? adminCategoryId
        )
        {
            var course = courseService.GetCourseDetailsFilteredByCategory(customisationId, centreId, adminCategoryId)!;
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
                $"Course {course.CustomisationId}",
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );
        }

        private static void SetUpDataTableColumns(
            CentreCustomPrompts customRegistrationPrompts,
            CourseAdminFields adminFields,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[] { new DataColumn("Last name"), new DataColumn("First name"), new DataColumn("Email") }
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
                    new DataColumn("Delegate ID"), new DataColumn("Enrolled", typeof(DateTime)),
                    new DataColumn("Last accessed", typeof(DateTime)), new DataColumn("Complete by", typeof(DateTime?)),
                    new DataColumn("Completed date", typeof(DateTime?)), new DataColumn("Logins", typeof(int)),
                    new DataColumn("Time (minutes)", typeof(int)), new DataColumn("Diagnostic score", typeof(int)),
                    new DataColumn("Assessments passed", typeof(int)), new DataColumn("Pass rate", typeof(double)),
                    new DataColumn("Active", typeof(bool)), new DataColumn("Removed date", typeof(DateTime?)),
                    new DataColumn("Locked", typeof(bool)),
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

            row["Last name"] = courseDelegate.LastName;
            row["First name"] = courseDelegate.FirstName;
            row["Email"] = courseDelegate.EmailAddress;

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

            row["Delegate ID"] = courseDelegate.CandidateNumber;
            row["Enrolled"] = courseDelegate.Enrolled.Date;
            row["Last accessed"] = courseDelegate.LastUpdated.Date;
            row["Complete by"] = courseDelegate.CompleteByDate?.Date;
            row["Completed date"] = courseDelegate.Completed?.Date;
            row["Logins"] = courseDelegate.LoginCount;
            row["Time (minutes)"] = courseDelegate.Duration;
            row["Diagnostic score"] = courseDelegate.DiagnosticScore;
            row["Assessments passed"] = courseDelegate.AttemptsPassed;
            row["Pass rate"] = courseDelegate.PassRate;
            row["Active"] = courseDelegate.Active;
            row["Removed date"] = courseDelegate.RemovedDate?.Date;
            row["Locked"] = courseDelegate.Locked;

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
    }
}
