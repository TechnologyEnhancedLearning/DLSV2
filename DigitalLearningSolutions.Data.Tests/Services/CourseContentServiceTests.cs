namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class CourseContentServiceTests
    {
        private CourseContentService courseContentService;
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseContentService>>();
            courseContentService = new CourseContentService(connection, logger);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }

        [Test]
        public void Get_course_content_of_partially_complete_course_should_return_course()
        {
            // When
            const int candidateId = 22044;
            const int customisationId = 4169;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4169,
                "Level 2 - Microsoft Excel 2010",
                "MOS Excel 2010 CORE",
                349,
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                false,
                null,
                0,
                true,
                85,
                90,
                0,
                null
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Viewing workbooks", 112, true, 12.5),
                    new CourseSection("Manipulating worksheets", 113, true, 20),
                    new CourseSection("Manipulating information", 114, true, 25),
                    new CourseSection("Using formulas", 115, true, 100 / 3.0),
                    new CourseSection("Using functions", 116, true, 400 / 7.0),
                    new CourseSection("Managing formulas and functions", 117, true, 0),
                    new CourseSection("Working with data", 118, true, 0),
                    new CourseSection("Formatting cells and worksheets", 119, true, 0),
                    new CourseSection("Formatting numbers", 120, true, 0),
                    new CourseSection("Working with charts", 121, true, 0),
                    new CourseSection("Working with illustrations", 122, true, 0),
                    new CourseSection("Collaborating with others", 123, true, 0),
                    new CourseSection("Preparing to print", 124, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_unstarted_course_should_return_course()
        {
            // When
            const int candidateId = 22044000;
            const int customisationId = 4169;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4169,
                "Level 2 - Microsoft Excel 2010",
                "MOS Excel 2010 CORE",
                349,
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                false,
                null,
                0,
                true,
                85,
                90,
                0,
                null
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Viewing workbooks", 112, true, 0),
                    new CourseSection("Manipulating worksheets", 113, true, 0),
                    new CourseSection("Manipulating information", 114, true, 0),
                    new CourseSection("Using formulas", 115, true, 0),
                    new CourseSection("Using functions", 116, true, 0),
                    new CourseSection("Managing formulas and functions", 117, true, 0),
                    new CourseSection("Working with data", 118, true, 0),
                    new CourseSection("Formatting cells and worksheets", 119, true, 0),
                    new CourseSection("Formatting numbers", 120, true, 0),
                    new CourseSection("Working with charts", 121, true, 0),
                    new CourseSection("Working with illustrations", 122, true, 0),
                    new CourseSection("Collaborating with others", 123, true, 0),
                    new CourseSection("Preparing to print", 124, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_course_without_learning_should_return_course()
        {
            // When
            const int customisationId = 4339;
            const int candidateId = 213382;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4339,
                "Level 2 - Microsoft PowerPoint 2007",
                "Working with Graphics and Multimedia",
                20,
                "NHS Highland",
                null,
                false,
                null,
                0,
                true,
                85,
                0,
                100,
                null
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Working in PowerPoint", 125, false, 0),
                    new CourseSection("Creating a presentation", 126, false, 0),
                    new CourseSection("Formatting slides", 127, false, 0),
                    new CourseSection("Working with text", 128, false, 0),
                    new CourseSection("Working with graphics and multimedia", 129, true, 20.0),
                    new CourseSection("Working with tables and charts", 130, false, 0),
                    new CourseSection("Using animations and transitions", 131, false, 0),
                    new CourseSection("Collaborating on presentations", 132, false, 0),
                    new CourseSection("Preparing presentations", 133, false, 0),
                    new CourseSection("Delivering presentations", 134, false, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_unstarted_course_without_learning_should_return_course()
        {
            // When
            const int customisationId = 4339;
            const int candidateId = 260132;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4339,
                "Level 2 - Microsoft PowerPoint 2007",
                "Working with Graphics and Multimedia",
                20,
                "NHS Highland",
                null,
                false,
                null,
                0,
                true,
                85,
                0,
                100,
                null
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Working in PowerPoint", 125, false, 0),
                    new CourseSection("Creating a presentation", 126, false, 0),
                    new CourseSection("Formatting slides", 127, false, 0),
                    new CourseSection("Working with text", 128, false, 0),
                    new CourseSection("Working with graphics and multimedia", 129, true, 0),
                    new CourseSection("Working with tables and charts", 130, false, 0),
                    new CourseSection("Using animations and transitions", 131, false, 0),
                    new CourseSection("Collaborating on presentations", 132, false, 0),
                    new CourseSection("Preparing presentations", 133, false, 0),
                    new CourseSection("Delivering presentations", 134, false, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_with_non_null_completed_date_should_return_completed_date()
        {
            // Given
            const int candidateId = 144100;
            const int customisationId = 4169;

            using (new TransactionScope())
            {
                // When
                var result = courseContentService.GetCourseContent(candidateId, customisationId);

                // Then
                var expectedCourse = new CourseContent(
                    4169,
                    "Level 2 - Microsoft Excel 2010",
                    "MOS Excel 2010 CORE",
                    349,
                    "Northumbria Healthcare NHS Foundation Trust",
                    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    false,
                    new DateTime(2016, 9, 15, 13, 1, 30, 623),
                    0,
                    true,
                    85,
                    90,
                    0,
                    null
                );
                expectedCourse.Sections.AddRange(
                    new[]
                    {
                        new CourseSection("Viewing workbooks", 112, true, 100.0),
                        new CourseSection("Manipulating worksheets", 113, true, 100.0),
                        new CourseSection("Manipulating information", 114, true, 100.0),
                        new CourseSection("Using formulas", 115, true, 100.0),
                        new CourseSection("Using functions", 116, true, 100.0),
                        new CourseSection("Managing formulas and functions", 117, true, 100.0),
                        new CourseSection("Working with data", 118, true, 100.0),
                        new CourseSection("Formatting cells and worksheets", 119, true, 100.0),
                        new CourseSection("Formatting numbers", 120, true, 100.0),
                        new CourseSection("Working with charts", 121, true, 100.0),
                        new CourseSection("Working with illustrations", 122, true, 100.0),
                        new CourseSection("Collaborating with others", 123, true, 100.0),
                        new CourseSection("Preparing to print", 124, true, 100.0)
                    }
                );
                result.Should().BeEquivalentTo(expectedCourse);
            }
        }

        [Test]
        public void Get_course_content_with_include_certification_should_return_include_certification()
        {
            // Given
            const int candidateId = 22044;
            const int customisationId = 4169;

            using (new TransactionScope())
            {
                // Then
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);
                var result = courseContentService.GetCourseContent(candidateId, customisationId);
                var expectedCourse = new CourseContent(
                    4169,
                    "Level 2 - Microsoft Excel 2010",
                    "MOS Excel 2010 CORE",
                    349,
                    "Northumbria Healthcare NHS Foundation Trust",
                    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    true,
                    null,
                    0,
                    true,
                    85,
                    90,
                    0,
                    null
                );
                expectedCourse.Sections.AddRange(
                    new[]
                    {
                        new CourseSection("Viewing workbooks", 112, true, 12.5),
                        new CourseSection("Manipulating worksheets", 113, true, 20),
                        new CourseSection("Manipulating information", 114, true, 25),
                        new CourseSection("Using formulas", 115, true, 100 / 3.0),
                        new CourseSection("Using functions", 116, true, 400 / 7.0),
                        new CourseSection("Managing formulas and functions", 117, true, 0),
                        new CourseSection("Working with data", 118, true, 0),
                        new CourseSection("Formatting cells and worksheets", 119, true, 0),
                        new CourseSection("Formatting numbers", 120, true, 0),
                        new CourseSection("Working with charts", 121, true, 0),
                        new CourseSection("Working with illustrations", 122, true, 0),
                        new CourseSection("Collaborating with others", 123, true, 0),
                        new CourseSection("Preparing to print", 124, true, 0)
                    }
                );
                result.Should().BeEquivalentTo(expectedCourse);
            }
        }

        [Test]
        public void Get_course_content_for_removed_and_refreshed_course_should_return_new_course()
        {
            // Given
            const int candidateId = 281358;
            const int customisationId = 8950;

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.UpdateSystemRefreshed(candidateId, customisationId, true);
                var result = courseContentService.GetCourseContent(candidateId, customisationId);

                // Then
                var expectedCourse = new CourseContent(
                    8950,
                    "Level 2 - Microsoft Word 2010",
                    "BSMHFT",
                    230,
                    "Birmingham & Solihull Mental Health Foundation Trust",
                    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    false,
                    null,
                    0,
                    true,
                    85,
                    85,
                    0,
                    null
                );
                expectedCourse.Sections.AddRange(
                    new[]
                    {
                        new CourseSection("Working with documents", 103, true, 0),
                        new CourseSection("Formatting content", 104, true, 0),
                        new CourseSection("Formatting documents", 105, true, 0),
                        new CourseSection("Illustrations and graphics", 106, true, 0),
                        new CourseSection("Using tables", 107, true, 0),
                        new CourseSection("Working with references", 108, true, 0),
                        new CourseSection("Proofing and working on documents with others", 109, true, 0),
                        new CourseSection("Sharing documents", 110, true, 0),
                        new CourseSection("Mass-mailing documents", 111, true, 0)
                    }
                );
                result.Should().BeEquivalentTo(expectedCourse);
            }
        }

        [Test]
        public void Get_course_content_for_inactive_course_should_return_null()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 24635;

            // When
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_course_content_for_removed_course_should_return_new_course()
        {
            // Given
            const int candidateId = 281358;
            const int customisationId = 8950;

            // When
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                8950,
                "Level 2 - Microsoft Word 2010",
                "BSMHFT",
                230,
                "Birmingham & Solihull Mental Health Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                false,
                null,
                0,
                true,
                85,
                85,
                0,
                null
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Working with documents", 103, true, 0),
                    new CourseSection("Formatting content", 104, true, 0),
                    new CourseSection("Formatting documents", 105, true, 0),
                    new CourseSection("Illustrations and graphics", 106, true, 0),
                    new CourseSection("Using tables", 107, true, 0),
                    new CourseSection("Working with references", 108, true, 0),
                    new CourseSection("Proofing and working on documents with others", 109, true, 0),
                    new CourseSection("Sharing documents", 110, true, 0),
                    new CourseSection("Mass-mailing documents", 111, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_for_refreshed_course_should_return_new_course()
        {
            // Given
            const int candidateId = 22044;
            const int customisationId = 4169;

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.UpdateSystemRefreshed(candidateId, customisationId, true);
                var result = courseContentService.GetCourseContent(candidateId, customisationId);

                // Then
                var expectedCourse = new CourseContent(
                    4169,
                    "Level 2 - Microsoft Excel 2010",
                    "MOS Excel 2010 CORE",
                    349,
                    "Northumbria Healthcare NHS Foundation Trust",
                    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    false,
                    null,
                    0,
                    true,
                    85,
                    90,
                    0,
                    null
                );
                expectedCourse.Sections.AddRange(
                    new[]
                    {
                        new CourseSection("Viewing workbooks", 112, true, 0),
                        new CourseSection("Manipulating worksheets", 113, true, 0),
                        new CourseSection("Manipulating information", 114, true, 0),
                        new CourseSection("Using formulas", 115, true, 0),
                        new CourseSection("Using functions", 116, true, 0),
                        new CourseSection("Managing formulas and functions", 117, true, 0),
                        new CourseSection("Working with data", 118, true, 0),
                        new CourseSection("Formatting cells and worksheets", 119, true, 0),
                        new CourseSection("Formatting numbers", 120, true, 0),
                        new CourseSection("Working with charts", 121, true, 0),
                        new CourseSection("Working with illustrations", 122, true, 0),
                        new CourseSection("Collaborating with others", 123, true, 0),
                        new CourseSection("Preparing to print", 124, true, 0)
                    }
                );
                result.Should().BeEquivalentTo(expectedCourse);
            }
        }

        [Test]
        public void Get_course_content_should_only_return_sections_with_tutorials_in_customisation()
        {
            // Given
            const int candidateId = 254480;
            const int customisationId = 24224;

            using (new TransactionScope())
            {
                // When
                var result = courseContentService.GetCourseContent(candidateId, customisationId);

                // Then
                var expectedCourse = new CourseContent(
                    24224,
                    "CMS Demonstration Course",
                    "Captivate Test",
                    13,
                    "Test Centre NHSD",
                    null,
                    false,
                    null,
                    1,
                    false,
                    85,
                    0,
                    100,
                    null
                );
                expectedCourse.Sections.AddRange(
                    new[]
                    {
                        new CourseSection("Dementia Awareness", 245, true, 0),
                    }
                );
                result.Should().BeEquivalentTo(expectedCourse);
            }
        }

        [TestCase(254480, 12589, 101, 285054)]
        [TestCase(1, 15853, 101, 173218)]
        [TestCase(254480, 24224, 101, null)]
        [TestCase(22044, 10059, 121, 100467)]
        [TestCase(11, 6226, 101, 89490)]
        public void Get_course_content_should_have_same_sections_as_stored_procedure(
            int candidateId,
            int customisationId,
            int centreId,
            int? progressId
        )
        {
            using (new TransactionScope())
            {
                // Given
                var validProgressId = progressId ?? courseContentTestHelper.CreateProgressId(customisationId, candidateId, centreId);

                var sectionIdsReturnedFromOldStoredProcedure = courseContentTestHelper
                    .SectionsFromOldStoredProcedure(validProgressId)
                    .Select(section => section.SectionID);

                // When
                var sectionIdsInCourseContent = courseContentService
                    .GetCourseContent(candidateId, customisationId)?
                    .Sections
                    .Select(section => section.Id);

                // Then
                sectionIdsInCourseContent.Should().BeEquivalentTo(sectionIdsReturnedFromOldStoredProcedure);
            }
        }

        [TestCase(254480, 12589)]
        [TestCase(1, 15853)]
        [TestCase(254480, 24224)]
        [TestCase(22044, 10059)]
        [TestCase(11, 6226)]
        [TestCase(207900, 274400)]
        [TestCase(213382, 4339)]
        [TestCase(286788, 23638)]
        public void Get_course_content_should_have_same_duration_as_stored_procedure(
            int candidateId,
            int customisationId
        )
        {
            // Given
            var durationFromOldStoredFunction =
                courseContentTestHelper.GetCustomisationDurationFromOldProcedure(customisationId);

            // When
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var formattedResult = courseContentTestHelper.FormatDurationLikeOldProcedure(
                result?.AverageDuration
            );
            formattedResult.Should().Be(durationFromOldStoredFunction);
        }


        [Test]
        public void Get_or_create_progress_id_should_return_progress_id_if_exists()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int centreId = 53;
            var initialDoesProgressExist = courseContentTestHelper.DoesProgressExist(candidateId, customisationId);

            using (new TransactionScope())
            {
                // When
                var result = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

                // Then
                initialDoesProgressExist.Should().BeTrue();
                const int expectedProgressId = 10;
                result.Should().Be(expectedProgressId);
            }
        }

        [Test]
        public void Get_or_create_progress_id_should_insert_new_progress_if_does_not_exist()
        {
            // Given
            const int candidateId = 187251;
            const int customisationId = 17468;
            const int centreId = 549;
            var initialDoesProgressExist = courseContentTestHelper.DoesProgressExist(candidateId, customisationId);

            using (new TransactionScope())
            {
                // When
                var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);
                var isProgressAdded = courseContentTestHelper.DoesProgressExist(candidateId, customisationId);

                //Then
                progressId.Should().NotBeNull();
                initialDoesProgressExist.Should().BeFalse();
                isProgressAdded.Should().BeTrue();
            }
        }

        [Test]
        public void Update_progress_should_not_increment_login_count_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(expectedLoginCount);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_duration_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(expectedDuration);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_login_count_if_session_time_is_before_first_submitted_time()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 8, 23);
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(expectedLoginCount);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_duration_if_session_time_is_before_first_submitted_time()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 8, 23);
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(expectedDuration);
            }
        }

        [Test]
        public void Update_progress_should_increment_login_count_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(initialLoginCount + 1);
            }
        }

        [Test]
        public void Update_progress_should_increment_duration_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(initialDuration + duration);
            }
        }

        [Test]
        public void Update_progress_should_update_submitted_time()
        {
            // Given
            const int progressId = 10;

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetSubmittedTime(progressId);

                // Then
                const int twoMinutesInMilliseconds = 120000;
                result.Should().BeCloseTo(DateTime.Now, twoMinutesInMilliseconds);
            }
        }
    }
}
