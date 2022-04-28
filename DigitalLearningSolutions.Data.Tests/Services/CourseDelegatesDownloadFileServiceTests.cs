namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;

    public class CourseDelegatesDownloadFileServiceTests
    {
        private const string CourseDelegateExportCurrentDataDownloadRelativeFilePath =
            "\\TestData\\CourseDelegateExportCurrentDataDownloadTest.xlsx";

        private const string CourseDelegateExportAllDataDownloadRelativeFilePath =
            "\\TestData\\CourseDelegateExportAllDataDownloadTest.xlsx";

        private readonly List<CourseDelegateForExport> courseDelegates = new List<CourseDelegateForExport>
        {
            new CourseDelegateForExport
            {
                CandidateNumber = "TM68",
                FirstName = "Philip",
                LastName = "Barber",
                EmailAddress = "mtc@.o",
                Active = true,
                Locked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2012, 05, 24),
                CompleteByDate = null,
                RemovedDate = null,
                Completed = null,
                AllAttempts = 1,
                AttemptsPassed = 0,
                Answer1 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                Answer2 = "xxxxxxx",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                CourseAnswer1 = null,
                CourseAnswer2 = null,
                CourseAnswer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                CandidateNumber = "ES1",
                FirstName = "Jonathan",
                LastName = "Bennett",
                EmailAddress = "slumrdaiehn.b@g",
                Active = true,
                Locked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2010, 09, 22),
                CompleteByDate = null,
                RemovedDate = null,
                Completed = new DateTime(2018, 03, 08),
                AllAttempts = 1,
                AttemptsPassed = 1,
                Answer1 = "Senior Implementation and Business Change Manager",
                Answer2 = "test2",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                CourseAnswer1 = null,
                CourseAnswer2 = null,
                CourseAnswer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                CandidateNumber = "NB8",
                FirstName = "Erik",
                LastName = "Griffin",
                EmailAddress = ".bcn@om",
                Active = true,
                Locked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2011, 03, 22),
                CompleteByDate = null,
                RemovedDate = null,
                Completed = new DateTime(2018, 03, 08),
                AllAttempts = 1,
                AttemptsPassed = 1,
                Answer1 = "xxxxxxxxxxx",
                Answer2 = "xxxxxxx",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                CourseAnswer1 = null,
                CourseAnswer2 = null,
                CourseAnswer3 = null,
                LoginCount = 1,
                Duration = 1,
                DiagnosticScore = 0,
            },
        };

        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDelegatesDataService courseDelegatesDataService = null!;
        private CourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseService courseService = null!;
        private ICentreRegistrationPromptsService registrationPromptsService = null!;

        [SetUp]
        public void Setup()
        {
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            courseDelegatesDataService = A.Fake<ICourseDelegatesDataService>();
            registrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            courseService = A.Fake<ICourseService>();

            courseDelegatesDownloadFileService = new CourseDelegatesDownloadFileService(
                courseDelegatesDataService,
                courseAdminFieldsService,
                registrationPromptsService,
                courseService
            );
        }

        [Test]
        public void GetDelegateDownloadFileForCourse_returns_expected_excel_data()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 1;
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + CourseDelegateExportCurrentDataDownloadRelativeFilePath
            );

            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourseForExport(customisationId, centreId))
                .Returns(courseDelegates);

            var centreRegistrationPrompts = new List<CentreRegistrationPrompt>
            {
                new CentreRegistrationPrompt(1, 1, "Role type", null, true),
                new CentreRegistrationPrompt(2, 2, "Manager", null, true),
                new CentreRegistrationPrompt(3, 3, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(4, 4, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(5, 5, "Contact telephone number", null, true),
            };
            A.CallTo(() => registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId))
                .Returns(new CentreRegistrationPrompts(centreId, centreRegistrationPrompts));

            var adminFields = new List<CourseAdminField>
            {
                new CourseAdminField(1, "Access Permissions", null),
            };
            A.CallTo(() => courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId))
                .Returns(new CourseAdminFields(customisationId, adminFields));

            // When
            var resultBytes = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(
                customisationId,
                centreId,
                null,
                null
            );
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }

        [Test]
        public void GetCourseDelegateDownloadFile_returns_expected_excel_data()
        {
            // Given
            const int categoryId = 1;
            const int centreId = 1;
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + CourseDelegateExportAllDataDownloadRelativeFilePath
            );

            A.CallTo(() => courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, categoryId)).Returns(
                new CentreCourseDetails
                {
                    Courses = new[]
                    {
                        new CourseStatisticsWithAdminFieldResponseCounts
                            { ApplicationName = "Course One", CustomisationId = 1 },
                        new CourseStatisticsWithAdminFieldResponseCounts
                            { ApplicationName = "Course Two", CustomisationId = 2 },
                    },
                }
            );

            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourseForExport(1, centreId))
                .Returns(courseDelegates);
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourseForExport(2, centreId))
                .Returns(courseDelegates.Where(c => c.CandidateNumber != "NB8"));

            var centreRegistrationPrompts = new List<CentreRegistrationPrompt>
            {
                new CentreRegistrationPrompt(1, 1, "Role type", null, true),
                new CentreRegistrationPrompt(2, 2, "Manager", null, true),
                new CentreRegistrationPrompt(3, 3, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(4, 4, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(5, 5, "Contact telephone number", null, true),
            };
            A.CallTo(() => registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId))
                .Returns(new CentreRegistrationPrompts(centreId, centreRegistrationPrompts));

            // When
            var resultBytes = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFile(
                centreId,
                categoryId,
                null,
                null,
                null
            );
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }
    }
}
