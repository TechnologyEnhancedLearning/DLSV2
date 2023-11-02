﻿namespace DigitalLearningSolutions.Web.Tests.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseDelegatesDownloadFileServiceTests
    {
        private const string CourseDelegateExportCurrentDataDownloadRelativeFilePath =
            "/TestData/CourseDelegateExportCurrentDataDownloadTest.xlsx";

        private const string CourseDelegateExportAllDataDownloadRelativeFilePath =
            "/TestData/CourseDelegateExportAllDataDownloadTest.xlsx";

        private readonly List<CourseDelegateForExport> courseDelegates = new List<CourseDelegateForExport>
        {
            new CourseDelegateForExport
            {
                ApplicationName = "Course One",
                CustomisationName = "v1",
                CandidateNumber = "TM68",
                DelegateFirstName = "Philip",
                DelegateLastName = "Barber",
                DelegateEmail = "mtc@.o",
                IsDelegateActive = true,
                IsProgressLocked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2012, 05, 24),
                CompleteBy = null,
                RemovedDate = null,
                Completed = null,
                AllAttempts = 1,
                AttemptsPassed = 0,
                RegistrationAnswer1 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                RegistrationAnswer2 = "xxxxxxx",
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                ApplicationName = "Course One",
                CustomisationName = "v1",
                CandidateNumber = "ES1",
                DelegateFirstName = "Jonathan",
                DelegateLastName = "Bennett",
                DelegateEmail = "slumrdaiehn.b@g",
                IsDelegateActive = true,
                IsProgressLocked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2010, 09, 22),
                CompleteBy = null,
                RemovedDate = null,
                Completed = new DateTime(2018, 03, 08),
                AllAttempts = 1,
                AttemptsPassed = 1,
                RegistrationAnswer1 = "Senior Implementation and Business Change Manager",
                RegistrationAnswer2 = "test2",
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                ApplicationName = "Course One",
                CustomisationName = "v1",
                CandidateNumber = "NB8",
                DelegateFirstName = "Erik",
                DelegateLastName = "Griffin",
                DelegateEmail = ".bcn@om",
                IsDelegateActive = true,
                IsProgressLocked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2011, 03, 22),
                CompleteBy = null,
                RemovedDate = null,
                Completed = new DateTime(2018, 03, 08),
                AllAttempts = 1,
                AttemptsPassed = 1,
                RegistrationAnswer1 = "xxxxxxxxxxx",
                RegistrationAnswer2 = "xxxxxxx",
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer3 = null,
                Answer1 = null,
                Answer2 = null,
                LoginCount = 1,
                Duration = 1,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                ApplicationName = "Course Two",
                CustomisationName = "v1",
                CandidateNumber = "TM68",
                DelegateFirstName = "Philip",
                DelegateLastName = "Barber",
                DelegateEmail = "mtc@.o",
                IsDelegateActive = true,
                IsProgressLocked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2012, 05, 24),
                CompleteBy = null,
                RemovedDate = null,
                Completed = null,
                AllAttempts = 1,
                AttemptsPassed = 0,
                RegistrationAnswer1 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                RegistrationAnswer2 = "xxxxxxx",
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
            new CourseDelegateForExport
            {
                ApplicationName = "Course Two",
                CustomisationName = "v1",
                CandidateNumber = "ES1",
                DelegateFirstName = "Jonathan",
                DelegateLastName = "Bennett",
                DelegateEmail = "slumrdaiehn.b@g",
                IsDelegateActive = true,
                IsProgressLocked = false,
                LastUpdated = new DateTime(2018, 03, 08),
                Enrolled = new DateTime(2010, 09, 22),
                CompleteBy = null,
                RemovedDate = null,
                Completed = new DateTime(2018, 03, 08),
                AllAttempts = 1,
                AttemptsPassed = 1,
                RegistrationAnswer1 = "Senior Implementation and Business Change Manager",
                RegistrationAnswer2 = "test2",
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                LoginCount = 2,
                Duration = 0,
                DiagnosticScore = 0,
            },
        };

        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDataService courseDataService = null!;
        private CourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseService courseService = null!;
        private ICentreRegistrationPromptsService registrationPromptsService = null!;

        [SetUp]
        public void Setup()
        {
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            courseDataService = A.Fake<ICourseDataService>();
            registrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            courseService = A.Fake<ICourseService>();
            
            courseDelegatesDownloadFileService = new CourseDelegatesDownloadFileService(
                courseDataService,
                courseAdminFieldsService,
                registrationPromptsService,
                courseService
            );
            
        }

        [Test]
        public async Task GetDelegateDownloadFileForCourse_returns_expected_excel_data()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 1;
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + CourseDelegateExportCurrentDataDownloadRelativeFilePath
            );

            A.CallTo(() => courseDataService.GetCourseDelegatesCountForExport(string.Empty,"SearchableName", "Ascending",
                   customisationId, centreId, null, null, null, null, null, null, null))
              .Returns(3);

            A.CallTo(() => courseDataService.GetCourseDelegatesForExport(string.Empty,0,250, "SearchableName", "Ascending",
                    customisationId, centreId, null, null, null, null, null, null, null))
               .Returns(courseDelegates.Where(c => c.ApplicationName == "Course One"));

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

            var resultBytes = await courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(string.Empty,0,250, "SearchableName", "Ascending",
                    customisationId, centreId, null, null, null, null, null, null, null
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
            const string sortDirection = GenericSortingHelper.Ascending;
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + CourseDelegateExportAllDataDownloadRelativeFilePath
            );

            A.CallTo(() => courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, categoryId,null,null,null,sortDirection)).Returns(
                new CentreCourseDetails
                {
                    Courses = new[]
                    {
                        new CourseStatisticsWithAdminFieldResponseCounts
                            { ApplicationName = "Course One", CustomisationName = "v1", CustomisationId = 1 },
                        new CourseStatisticsWithAdminFieldResponseCounts
                            { ApplicationName = "Course Two", CustomisationName = "v1", CustomisationId = 2 },
                    },
                }
            );

            A.CallTo(() => courseDataService.GetDelegatesOnCourseForExport(1, centreId))
                .Returns(courseDelegates.Where(c => c.ApplicationName == "Course One"));
            A.CallTo(() => courseDataService.GetDelegatesOnCourseForExport(2, centreId))
                .Returns(courseDelegates.Where(c => c.ApplicationName == "Course Two"));

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
