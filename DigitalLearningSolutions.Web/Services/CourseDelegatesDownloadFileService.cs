namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface ICourseDelegatesDownloadFileService
    {
        public byte[] GetCourseDelegateDownloadFileForCourse(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3
        );

        public byte[] GetActivityDelegateDownloadFile(
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? filterString,
            string courseTopic,
            string hasAdminFields,
            string categoryName,
            string isActive,
            string isCourse,
            string isSelfAssessment,
            string? sortBy,
            string sortDirection = GenericSortingHelper.Ascending
        );
    }

    public class CourseDelegatesDownloadFileService : ICourseDelegatesDownloadFileService
    {
        private const string CourseName = "Course name";
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
        private const string SelfAssessmentName = "Self assessment name";
        private const string PRN = "PRN";
        private const string Submitted = "Submitted";
        private const string SignedOff = "Signed off";
        private const string Launches = "Launches";

        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;
        private readonly ICentreRegistrationPromptsService registrationPromptsService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public CourseDelegatesDownloadFileService(
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            ICentreRegistrationPromptsService registrationPromptsService,
            ICourseService courseService,
            ISelfAssessmentDataService selfAssessmentDataService
        )
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.registrationPromptsService = registrationPromptsService;
            this.courseService = courseService;
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public byte[] GetCourseDelegateDownloadFileForCourse(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3
        )
        {
            using var workbook = new XLWorkbook();

            var adminFields = courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId);

            var customRegistrationPrompts = registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            int resultCount = courseDataService.GetCourseDelegatesCountForExport(searchString ?? string.Empty, sortBy, sortDirection,
                    customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);


            int page = 1;
            int totalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);

            List<CourseDelegateForExport> courseDelegates = new List<CourseDelegateForExport>();

            while (totalPages >= page)
            {
                offSet = ((page - 1) * itemsPerPage);

                courseDelegates.AddRange(this.courseDataService.GetCourseDelegatesForExport(searchString ?? string.Empty, offSet, itemsPerPage, sortBy, sortDirection,
                    customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3));
                page++;
            }

            PopulateCourseDelegatesSheetForCourse(
                workbook,
                adminFields,
                customRegistrationPrompts,
                courseDelegates,
                customisationId
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GetActivityDelegateDownloadFile(
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? filterString,
            string courseTopic,
            string hasAdminFields,
            string categoryName,
            string isActive,
            string isCourse,
            string isSelfassessment,
            string? sortBy,
            string sortDirection = GenericSortingHelper.Ascending
        )
        {
            using var workbook = new XLWorkbook();

            PopulateActivityDelegatesSheetForExportAll(
                workbook,
                centreId,
                adminCategoryId,
                searchString,
                filterString,
                courseTopic,
                hasAdminFields,
                categoryName,
                isActive,
                isCourse,
                isSelfassessment,
                sortBy,
                sortDirection
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateCourseDelegatesSheetForCourse(
            IXLWorkbook workbook,
           CourseAdminFields adminFields,
           CentreRegistrationPrompts customRegistrationPrompts,
           IEnumerable<CourseDelegateForExport> courseDelegates,
           int customisationId
        )
        {
            var dataTable = new DataTable();

            SetUpDataTableColumns(customRegistrationPrompts, adminFields, dataTable);

            foreach (var courseDelegate in courseDelegates)
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

        private void PopulateActivityDelegatesSheetForExportAll(
            IXLWorkbook workbook,
            int centreId,
            int? adminCategoryId,
            string? searchString,
            string? filterString,
            string courseTopic,
            string hasAdminFields,
            string categoryName,
            string isActive,
            string isCourse,
            string isSelfAssessment,
            string? sortBy,
            string sortDirection
        )
        {
            var sheet = workbook.Worksheets.Add("Course Delegates");

            // Set sheet to have outlining expand buttons at the top of the expanded section.
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            var customRegistrationPrompts =
                registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses = new CourseStatisticsWithAdminFieldResponseCounts[] { };
            IEnumerable<DelegateAssessmentStatistics> selfAssessments = new DelegateAssessmentStatistics[] { };

            if (isCourse == "Any" && isSelfAssessment == "Any")
            {
                courses = GetCoursesToExport(centreId, adminCategoryId, searchString, sortBy, filterString, sortDirection);
                if (courseTopic == "Any" && hasAdminFields == "Any")
                    selfAssessments = courseService.GetDelegateAssessments(searchString, centreId, categoryName, isActive);
            }
            if (isCourse == "true")
                courses = GetCoursesToExport(centreId, adminCategoryId, searchString, sortBy, filterString, sortDirection);
            if (isSelfAssessment == "true" && courseTopic == "Any" && hasAdminFields == "Any")
                selfAssessments = courseService.GetDelegateAssessments(searchString, centreId, categoryName, isActive);

            if (selfAssessments.Any())
            {
                selfAssessments = UpdateOrderSelfAssessments(selfAssessments, sortBy, sortDirection);
            }

            var emptyTable = new DataTable();

            //export courses
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

            //export self assessments
            sheet = workbook.Worksheets.Add("Self Assessment Delegates");
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            emptyTable = new DataTable();
            SetUpDataTableColumnsForSelfAssessment(customRegistrationPrompts, emptyTable);
            headerTable = sheet.Cell(1, 1).InsertTable(emptyTable);
            headerTable.Theme = XLTableTheme.None;

            foreach (var selfAssessment in selfAssessments)
            {
                AddSelfAssessmentToSheet(sheet, centreId, selfAssessment, customRegistrationPrompts);
            }

            sheet.Columns(1, sheet.LastColumnUsed().RangeAddress.FirstAddress.ColumnNumber).AdjustToContents();

            if (GetNextEmptyRowNumber(sheet) > 2)
            {
                headerTable.Resize(1, 1, sheet.LastRowUsed().RowNumber(), sheet.LastColumnUsed().ColumnNumber());
                sheet.CollapseRows();
            }

            FormatWorksheetColumns(workbook, emptyTable, 2);
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
            var details = courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, adminCategoryId, searchString, sortBy, filterString, sortDirection);
            var filteredCourses = FilteringHelper.FilterItems(details.Courses.AsQueryable(), filterString);
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

        private void AddSelfAssessmentToSheet(
            IXLWorksheet sheet,
            int centreId,
            DelegateAssessmentStatistics selfAssessment,
            CentreRegistrationPrompts customRegistrationPrompts
        )
        {
            var selfAssessmentNameCell = sheet.Cell(GetNextEmptyRowNumber(sheet), 1);
            selfAssessmentNameCell.Value = selfAssessment.SearchableName;
            selfAssessmentNameCell.Style.Font.Bold = true;

            var sortedSelfAssessmentDelegates =
                GenericSortingHelper.SortAllItems(
                    selfAssessmentDataService.GetDelegatesOnSelfAssessmentForExport(selfAssessment.SelfAssessmentId, centreId)
                        .AsQueryable(),
                    nameof(SelfAssessmentDelegate.FullNameForSearchingSorting),
                    GenericSortingHelper.Ascending
                );

            var dataTable = new DataTable();

            SetUpDataTableColumnsForSelfAssessment(customRegistrationPrompts, dataTable);

            foreach (var selfAssessmentDelegate in sortedSelfAssessmentDelegates)
            {
                AddDelegateToDataTableForSelfAssessment(dataTable, selfAssessment.Name, selfAssessmentDelegate, customRegistrationPrompts);
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

        private static void SetUpDataTableColumnsForSelfAssessment(
            CentreRegistrationPrompts registrationRegistrationPrompts,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[] { new DataColumn(SelfAssessmentName), new DataColumn(LastName), new DataColumn(FirstName), new DataColumn(Email), new DataColumn(PRN) }
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
                    new DataColumn(CompleteBy), new DataColumn(Submitted), new DataColumn(SignedOff),
                    new DataColumn(Launches),
                }
            );
        }

        private static void SetUpCommonDataTableColumns(
            CentreRegistrationPrompts registrationRegistrationPrompts,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[] { new DataColumn(CourseName), new DataColumn(LastName), new DataColumn(FirstName), new DataColumn(Email) }
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
            row[CourseName] = courseDelegate.CourseName;
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

        private static void AddDelegateToDataTableForSelfAssessment(
            DataTable dataTable,
            string? selfAssessmentName,
            SelfAssessmentDelegate selfAssessmentDelegate,
            CentreRegistrationPrompts registrationRegistrationPrompts
        )
        {
            var row = dataTable.NewRow();

            row[SelfAssessmentName] = selfAssessmentName;
            row[LastName] = selfAssessmentDelegate.DelegateLastName;
            row[FirstName] = selfAssessmentDelegate.DelegateFirstName;
            row[Email] = selfAssessmentDelegate.DelegateEmail;
            row[PRN] = selfAssessmentDelegate.ProfessionalRegistrationNumber;

            foreach (var prompt in registrationRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        selfAssessmentDelegate.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        selfAssessmentDelegate.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }
            row[DelegateId] = selfAssessmentDelegate.CandidateNumber;
            row[Enrolled] = selfAssessmentDelegate.StartedDate.Date;
            row[LastAccessed] = selfAssessmentDelegate.LastAccessed?.Date;
            row[CompleteBy] = selfAssessmentDelegate.CompleteBy?.Date;
            row[Submitted] = selfAssessmentDelegate.SubmittedDate?.Date;
            row[SignedOff] = selfAssessmentDelegate.SignedOff?.Date;
            row[Launches] = selfAssessmentDelegate.LaunchCount;

            dataTable.Rows.Add(row);
        }

        private static void FormatWorksheetColumns(IXLWorkbook workbook, DataTable dataTable, int workSheetNumber = 1)
        {
            var dateColumns = new[] { Enrolled, LastAccessed, CompleteBy, CompletedDate, RemovedDate, Submitted, SignedOff };
            foreach (var columnName in dateColumns)
            {
                if (dataTable.Columns.Contains(columnName))
                    ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.DateTime, workSheetNumber);
            }

            var numberColumns = new[] { Logins, TimeMinutes, DiagnosticScore, AssessmentsPassed, PassRate, Launches };
            foreach (var columnName in numberColumns)
            {
                if (dataTable.Columns.Contains(columnName))
                    ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Number, workSheetNumber);
            }

            var boolColumns = new[] { Active, Locked };
            foreach (var columnName in boolColumns)
            {
                if (dataTable.Columns.Contains(columnName))
                    ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean, workSheetNumber);
            }
        }

        private IEnumerable<DelegateAssessmentStatistics> UpdateOrderSelfAssessments(IEnumerable<DelegateAssessmentStatistics> selfAssessments, string sortBy, string sortDirection)
        {
            foreach (var selfAssessment in selfAssessments)
            {
                selfAssessment.CompletedCount = selfAssessment.SubmittedSignedOffCount;
            }

            if (sortBy == "InProgressCount")
            {
                selfAssessments = sortDirection == "Ascending"
                            ? selfAssessments.OrderBy(x => x.InProgressCount).ThenBy(n => n.SearchableName).ToList()
                            : selfAssessments.OrderByDescending(x => x.InProgressCount).ThenBy(n => n.SearchableName).ToList();
            }
            else if (sortBy == "CompletedCount")
            {
                selfAssessments = sortDirection == "Ascending"
                            ? selfAssessments.OrderBy(x => x.CompletedCount).ThenBy(n => n.SearchableName).ToList()
                            : selfAssessments.OrderByDescending(x => x.CompletedCount).ThenBy(n => n.SearchableName).ToList();
            }
            else
            {
                selfAssessments = sortDirection == "Ascending"
                            ? selfAssessments.OrderBy(x => x.SearchableName).ToList()
                            : selfAssessments.OrderByDescending(x => x.SearchableName).ToList();
            }
            return selfAssessments;
        }

    }
}
