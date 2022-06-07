namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseDelegatesDownloadFileService
    {
        public byte[] GetCourseDelegateDownloadFileForCourse(
            int customisationId,
            int centreId,
            string? sortBy,
            string? filterString,
            string sortDirection = GenericSortingHelper.Ascending
        );

        public byte[] GetCourseDelegateDownloadFile(
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection = GenericSortingHelper.Ascending
        );
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
        private const string AdminFieldOne = "Admin field 1";
        private const string AdminFieldTwo = "Admin field 2";
        private const string AdminFieldThree = "Admin field 3";

        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;
        private readonly ICentreRegistrationPromptsService registrationPromptsService;

        public CourseDelegatesDownloadFileService(
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            ICentreRegistrationPromptsService registrationPromptsService,
            ICourseService courseService
        )
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.registrationPromptsService = registrationPromptsService;
            this.courseService = courseService;
        }

        public byte[] GetCourseDelegateDownloadFileForCourse(
            int customisationId,
            int centreId,
            string? sortBy,
            string? filterString,
            string sortDirection = GenericSortingHelper.Ascending
        )
        {
            using var workbook = new XLWorkbook();

            PopulateCourseDelegatesSheetForCourse(
                workbook,
                customisationId,
                centreId,
                sortBy,
                filterString,
                sortDirection
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GetCourseDelegateDownloadFile(
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection = GenericSortingHelper.Ascending
        )
        {
            using var workbook = new XLWorkbook();

            PopulateCourseDelegatesSheetForExportAll(
                workbook,
                centreId,
                adminCategoryId,
                searchString,
                sortBy,
                filterString,
                sortDirection
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateCourseDelegatesSheetForCourse(
            IXLWorkbook workbook,
            int customisationId,
            int centreId,
            string? sortBy,
            string? filterString,
            string sortDirection
        )
        {
            var adminFields = courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId);

            var customRegistrationPrompts = registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            var courseDelegates = courseDataService.GetDelegatesOnCourseForExport(customisationId, centreId)
                .ToList();

            var filteredCourseDelegates =
                FilteringHelper.FilterItems(courseDelegates.AsQueryable(), filterString).ToList();
            var sortedCourseDelegates =
                GenericSortingHelper.SortAllItems(
                    filteredCourseDelegates.AsQueryable(),
                    sortBy ?? nameof(CourseDelegateForExport.FullNameForSearchingSorting),
                    sortDirection
                );

            var dataTable = new DataTable();

            SetUpDataTableColumns(customRegistrationPrompts, adminFields, dataTable);

            foreach (var courseDelegate in sortedCourseDelegates)
            {
                AddDelegateToDataTable(dataTable, courseDelegate, customRegistrationPrompts, adminFields);
            }

            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                $"Course {customisationId}",
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatWorksheetColumns(workbook, dataTable);
        }

        private void PopulateCourseDelegatesSheetForExportAll(
            IXLWorkbook workbook,
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        )
        {
            var sheet = workbook.Worksheets.Add("Course Delegates");

            // Set sheet to have outlining expand buttons at the top of the expanded section.
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            var customRegistrationPrompts =
                registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            var courses = GetCoursesToExport(
                centreId,
                adminCategoryId,
                searchString,
                sortBy,
                filterString,
                sortDirection
            );

            var emptyTable = new DataTable();
            SetUpDataTableColumnsForExportAll(customRegistrationPrompts, emptyTable);
            var headerTable = sheet.Cell(1, 1).InsertTable(emptyTable);
            headerTable.Theme = XLTableTheme.None;

            foreach (var course in courses)
            {
                AddCourseToSheet(sheet, centreId, course, customRegistrationPrompts);
            }

            sheet.Columns(1, sheet.LastColumnUsed().RangeAddress.FirstAddress.ColumnNumber).AdjustToContents();

            if (GetNextEmptyRowNumber(sheet) > 2)
            {
                headerTable.Resize(1, 1, sheet.LastRowUsed().RowNumber(), sheet.LastColumnUsed().ColumnNumber());
                sheet.CollapseRows();
            }

            FormatWorksheetColumns(workbook, emptyTable);
        }

        private IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> GetCoursesToExport(
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        )
        {
            var details = courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, adminCategoryId);
            var searchedCourses = GenericSearchHelper.SearchItems(details.Courses, searchString);
            var filteredCourses = FilteringHelper.FilterItems(searchedCourses.AsQueryable(), filterString);
            var sortedCourses = GenericSortingHelper.SortAllItems(
                filteredCourses.AsQueryable(),
                sortBy ?? nameof(CourseStatisticsWithAdminFieldResponseCounts.CourseName),
                sortDirection
            );
            return sortedCourses;
        }

        private void AddCourseToSheet(
            IXLWorksheet sheet,
            int centreId,
            Course course,
            CentreRegistrationPrompts customRegistrationPrompts
        )
        {
            var courseNameCell = sheet.Cell(GetNextEmptyRowNumber(sheet), 1);
            courseNameCell.Value = course.CourseName;
            courseNameCell.Style.Font.Bold = true;

            var sortedCourseDelegates =
                GenericSortingHelper.SortAllItems(
                    courseDataService.GetDelegatesOnCourseForExport(course.CustomisationId, centreId)
                        .AsQueryable(),
                    nameof(CourseDelegateForExport.FullNameForSearchingSorting),
                    GenericSortingHelper.Ascending
                );

            var dataTable = new DataTable();

            SetUpDataTableColumnsForExportAll(customRegistrationPrompts, dataTable);

            foreach (var courseDelegate in sortedCourseDelegates)
            {
                AddDelegateToDataTableForExportAll(dataTable, courseDelegate, customRegistrationPrompts);
            }

            var insertedDataRange = sheet.Cell(GetNextEmptyRowNumber(sheet), 1).InsertData(dataTable.Rows);
            if (dataTable.Rows.Count > 0)
            {
                sheet.Rows(insertedDataRange.FirstRow().RowNumber(), insertedDataRange.LastRow().RowNumber())
                    .Group(true);
            }
        }

        private static int GetNextEmptyRowNumber(IXLWorksheet sheet)
        {
            return sheet.LastRowUsed().RowNumber() + 1;
        }

        private static void SetUpDataTableColumns(
            CentreRegistrationPrompts registrationRegistrationPrompts,
            CourseAdminFields adminFields,
            DataTable dataTable
        )
        {
            SetUpCommonDataTableColumns(registrationRegistrationPrompts, dataTable);

            foreach (var prompt in adminFields.AdminFields)
            {
                dataTable.Columns.Add(
                    !dataTable.Columns.Contains(prompt.PromptText)
                        ? prompt.PromptText
                        : $"{prompt.PromptText} (Prompt {prompt.PromptNumber})"
                );
            }
        }

        private static void SetUpDataTableColumnsForExportAll(
            CentreRegistrationPrompts registrationRegistrationPrompts,
            DataTable dataTable
        )
        {
            SetUpCommonDataTableColumns(registrationRegistrationPrompts, dataTable);

            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(AdminFieldOne),
                    new DataColumn(AdminFieldTwo),
                    new DataColumn(AdminFieldThree),
                }
            );
        }

        private static void SetUpCommonDataTableColumns(
            CentreRegistrationPrompts registrationRegistrationPrompts,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[] { new DataColumn(LastName), new DataColumn(FirstName), new DataColumn(Email) }
            );

            foreach (var prompt in registrationRegistrationPrompts.CustomPrompts)
            {
                dataTable.Columns.Add(
                    !dataTable.Columns.Contains(prompt.PromptText)
                        ? prompt.PromptText
                        : $"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"
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
        }

        private static void AddDelegateToDataTable(
            DataTable dataTable,
            CourseDelegateForExport courseDelegate,
            CentreRegistrationPrompts registrationRegistrationPrompts,
            CourseAdminFields adminFields
        )
        {
            var row = dataTable.NewRow();

            SetCommonRowValues(dataTable, courseDelegate, registrationRegistrationPrompts, row);

            foreach (var prompt in adminFields.AdminFields)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.PromptNumber})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.PromptNumber})"] =
                        courseDelegate.DelegateCourseAdminFields[prompt.PromptNumber - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        courseDelegate.DelegateCourseAdminFields[prompt.PromptNumber - 1];
                }
            }

            dataTable.Rows.Add(row);
        }

        private static void AddDelegateToDataTableForExportAll(
            DataTable dataTable,
            CourseDelegateForExport courseDelegate,
            CentreRegistrationPrompts registrationRegistrationPrompts
        )
        {
            var row = dataTable.NewRow();

            SetCommonRowValues(dataTable, courseDelegate, registrationRegistrationPrompts, row);

            row[AdminFieldOne] = courseDelegate.Answer1;
            row[AdminFieldTwo] = courseDelegate.Answer2;
            row[AdminFieldThree] = courseDelegate.Answer3;

            dataTable.Rows.Add(row);
        }

        private static void SetCommonRowValues(
            DataTable dataTable,
            CourseDelegateForExport courseDelegate,
            CentreRegistrationPrompts registrationRegistrationPrompts,
            DataRow row
        )
        {
            row[LastName] = courseDelegate.DelegateLastName;
            row[FirstName] = courseDelegate.DelegateFirstName;
            row[Email] = courseDelegate.DelegateEmail;

            foreach (var prompt in registrationRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        courseDelegate.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        courseDelegate.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }

            row[DelegateId] = courseDelegate.CandidateNumber;
            row[Enrolled] = courseDelegate.Enrolled.Date;
            row[LastAccessed] = courseDelegate.LastUpdated.Date;
            row[CompleteBy] = courseDelegate.CompleteBy?.Date;
            row[CompletedDate] = courseDelegate.Completed?.Date;
            row[Logins] = courseDelegate.LoginCount;
            row[TimeMinutes] = courseDelegate.Duration;
            row[DiagnosticScore] = courseDelegate.DiagnosticScore;
            row[AssessmentsPassed] = courseDelegate.AttemptsPassed;
            row[PassRate] = courseDelegate.PassRate;
            row[Active] = courseDelegate.IsDelegateActive;
            row[RemovedDate] = courseDelegate.RemovedDate?.Date;
            row[Locked] = courseDelegate.IsProgressLocked;
        }

        private static void FormatWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            var dateColumns = new[] { Enrolled, LastAccessed, CompleteBy, CompletedDate, RemovedDate };
            foreach (var columnName in dateColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.DateTime);
            }

            var numberColumns = new[] { Logins, TimeMinutes, DiagnosticScore, AssessmentsPassed, PassRate };
            foreach (var columnName in numberColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Number);
            }

            var boolColumns = new[] { Active, Locked };
            foreach (var columnName in boolColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean);
            }
        }
    }
}
