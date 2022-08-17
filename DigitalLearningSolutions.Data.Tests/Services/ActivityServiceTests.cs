namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ActivityServiceTests
    {
        public const string ActivityDataDownloadRelativeFilePath = "\\TestData\\ActivityDataDownloadTest.xlsx";
        private IActivityDataService activityDataService = null!;
        private IActivityService activityService = null!;
        private ICourseCategoriesDataService courseCategoriesDataService = null!;
        private ICourseDataService courseDataService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;

        [SetUp]
        public void SetUp()
        {
            activityDataService = A.Fake<IActivityDataService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            courseDataService = A.Fake<ICourseDataService>();
            activityService = new ActivityService(
                activityDataService,
                jobGroupsDataService,
                courseCategoriesDataService,
                courseDataService
            );
        }

        [Test]
        [TestCase(ReportInterval.Days, 732, "2014-6-22", "2016-6-22", "2015-12-22")]
        [TestCase(ReportInterval.Months, 25, "2014-6-01", "2016-6-01", "2015-12-01")]
        [TestCase(ReportInterval.Quarters, 9, "2014-4-01", "2016-4-01", "2015-10-01")]
        [TestCase(ReportInterval.Years, 3, "2014-1-01", "2016-1-01", "2015-1-01")]
        public void GetFilteredActivity_correctly_groups_activity(
            ReportInterval interval,
            int expectedSlotCount,
            string expectedStartDate,
            string expectedFinalDate,
            string expectedLogDateForCompletion
        )
        {
            // given
            var expectedActivityResult = new List<ActivityLog>
            {
                new ActivityLog
                {
                    Completed = true,
                    Evaluated = false,
                    Registered = false,
                    LogDate = DateTime.Parse("2015-12-22"),
                    LogYear = 2015,
                    LogQuarter = 4,
                    LogMonth = 12,
                },
            };
            GivenActivityDataServiceReturnsSpecifiedResult(expectedActivityResult);

            var filterData = new ActivityFilterData(
                DateTime.Parse("2014-6-22"),
                DateTime.Parse("2016-6-22"),
                null,
                null,
                null,
                CourseFilterType.None,
                interval
            );

            // when
            var result = activityService.GetFilteredActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                ValidatePeriodData(result.First(), expectedStartDate, interval, 0, 0, 0);
                ValidatePeriodData(result.Last(), expectedFinalDate, interval, 0, 0, 0);
                ValidatePeriodData(
                    result.Single(p => p.Completions == 1),
                    expectedLogDateForCompletion,
                    interval,
                    0,
                    1,
                    0
                );

                result.Count.Should().Be(expectedSlotCount);
                result.All(p => p.Evaluations == 0 && p.Registrations == 0).Should().BeTrue();
                result.All(p => p.DateInformation.Interval == interval).Should().BeTrue();
            }
        }

        [Test]
        public void GetFilteredActivity_returns_empty_slots_with_no_activity()
        {
            // given
            var filterData = new ActivityFilterData(
                DateTime.Parse("2115-6-22"),
                DateTime.Parse("2116-9-22"),
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Months
            );
            A.CallTo(
                    () => activityDataService.GetFilteredActivity(
                        A<int>._,
                        A<DateTime>._,
                        A<DateTime>._,
                        A<int?>._,
                        A<int?>._,
                        A<int?>._
                    )
                )
                .Returns(new List<ActivityLog>());

            // when
            var result = activityService.GetFilteredActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                result.Count.Should().Be(16);
                result.All(p => p.Completions == 0 && p.Evaluations == 0 && p.Registrations == 0).Should().BeTrue();
            }
        }

        [Test]
        public void GetFilteredActivity_requests_activity_with_correct_parameters()
        {
            // given
            var filterData = new ActivityFilterData(
                DateTime.Parse("2115-6-22"),
                DateTime.Parse("2116-9-22"),
                1,
                2,
                3,
                CourseFilterType.CourseCategory,
                ReportInterval.Months
            );

            // when
            activityService.GetFilteredActivity(101, filterData);

            // then
            A.CallTo(
                    () => activityDataService.GetFilteredActivity(
                        101,
                        filterData.StartDate,
                        filterData.EndDate,
                        filterData.JobGroupId,
                        filterData.CourseCategoryId,
                        null
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void GetFilterOptions_returns_expected_job_groups()
        {
            // Given
            var expectedJobGroups = JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical().ToList();

            const int centreId = 1;
            const int categoryId = 1;
            GivenDataServicesReturnData(centreId, categoryId);

            // When
            var result = activityService.GetFilterOptions(centreId, categoryId);

            // Then
            result.JobGroups.Should().BeEquivalentTo(expectedJobGroups);
        }

        [Test]
        public void GetFilterOptions_returns_expected_categories()
        {
            // Given
            var expectedCategories = new[] { (1, "Category 1"), (2, "Category 2") };

            const int centreId = 1;
            const int categoryId = 1;
            GivenDataServicesReturnData(centreId, categoryId);

            // When
            var result = activityService.GetFilterOptions(centreId, categoryId);

            // Then
            result.Categories.Should().BeEquivalentTo(expectedCategories);
        }

        [Test]
        public void GetFilterOptions_returns_distinct_courses_in_active_status_then_alphabetical_order()
        {
            // Given
            var expectedCourses = new[]
                { (2, "B Course"), (1, "C Course"), (4, "Inactive - A Course"), (3, "Inactive - D Course") };

            const int centreId = 1;
            const int categoryId = 1;
            GivenDataServicesReturnData(centreId, categoryId);

            // When
            var result = activityService.GetFilterOptions(centreId, categoryId);

            // Then
            result.Courses.Should().BeEquivalentTo(expectedCourses);
        }

        [Test]
        public void GetFilterNames_returns_all_with_all_ids_null()
        {
            // Given
            var filterData = new ActivityFilterData(
                DateTime.Now,
                null,
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Days
            );
            const string all = "All";

            // When
            var result = activityService.GetFilterNames(filterData);

            // Then
            result.jobGroupName.Should().Be(all);
            result.courseCategoryName.Should().Be(all);
            result.courseName.Should().Be(all);
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => courseCategoriesDataService.GetCourseCategoryName(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void GetFilterNames_returns_expected_job_group_name_with_non_null_job_group_id_filter()
        {
            // Given
            var filterData = new ActivityFilterData(
                DateTime.Now,
                null,
                1,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Days
            );
            const string job = "Job";
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(A<int>._)).Returns(job);

            // When
            var result = activityService.GetFilterNames(filterData);

            // Then
            result.jobGroupName.Should().Be(job);
        }

        [Test]
        public void GetFilterNames_returns_expected_category_name_with_non_null_category_filter()
        {
            // Given
            var filterData = new ActivityFilterData(
                DateTime.Now,
                null,
                null,
                1,
                null,
                CourseFilterType.CourseCategory,
                ReportInterval.Days
            );
            const string category = "Category";
            A.CallTo(() => courseCategoriesDataService.GetCourseCategoryName(A<int>._)).Returns(category);

            // When
            var result = activityService.GetFilterNames(filterData);

            // Then
            result.courseCategoryName.Should().Be(category);
        }

        [Test]
        public void GetFilterNames_returns_expected_filter_names_with_non_null_course_filter()
        {
            // Given
            var filterData = new ActivityFilterData(
                DateTime.Now,
                null,
                null,
                null,
                1,
                CourseFilterType.Course,
                ReportInterval.Days
            );
            const string course = "Course";
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(A<int>._))
                .Returns(new CourseNameInfo { ApplicationName = course });

            // When
            var result = activityService.GetFilterNames(filterData);

            // Then
            result.courseName.Should().Be(course);
        }

        private void GivenDataServicesReturnData(int centreId, int categoryId)
        {
            var jobGroups = JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical();
            var categories = new List<Category>
            {
                new Category { CourseCategoryID = 1, CategoryName = "Category 1" },
                new Category { CourseCategoryID = 2, CategoryName = "Category 2" },
            };
            var availableCourses = new List<CourseAssessmentDetails>
            {
                new CourseAssessmentDetails { CustomisationId = 1, ApplicationName = "C Course", Active = true },
                new CourseAssessmentDetails { CustomisationId = 2, ApplicationName = "B Course", Active = true },
            };
            var historicalCourses = new List<Course>
            {
                new Course { CustomisationId = 1, ApplicationName = "C Course", Active = true },
                new Course { CustomisationId = 3, ApplicationName = "D Course", Active = false },
                new Course { CustomisationId = 4, ApplicationName = "A Course", Active = false },
            };
            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(jobGroups);
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId))
                .Returns(categories);
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(availableCourses);
            A.CallTo(() => courseDataService.GetCoursesEverUsedAtCentreByCategory(centreId, categoryId))
                .Returns(historicalCourses);
        }

        [Test]
        public void GetActivityDataFileForCentre_returns_expected_excel_data()
        {
            // given
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + ActivityDataDownloadRelativeFilePath
            );
            GivenActivityDataServiceReturnsDataInExampleSheet();

            var filterData = new ActivityFilterData(
                DateTime.Parse("2020-9-1"),
                DateTime.Parse("2021-9-1"),
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Months
            );

            // when
            var resultBytes = activityService.GetActivityDataFileForCentre(101, filterData);

            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_null_for_start_date_before_activity_start()
        {
            // given
            var startDateString = "2012-06-06";
            var endDateString = "2021-06-06";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2012-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange.Should().BeNull();
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_null_for_end_date_before_start_date()
        {
            // given
            var startDateString = "2022-06-06";
            var endDateString = "2021-06-06";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2000-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange.Should().BeNull();
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_null_for_end_date_in_future()
        {
            // given
            var startDateString = "2012-06-06";
            var endDateString = "3021-06-06";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2000-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange.Should().BeNull();
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_null_for_invalid_start_date()
        {
            // given
            var startDateString = "once upon a time";
            var endDateString = "2021-06-06";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2000-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange.Should().BeNull();
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_tuple_for_invalid_end_date()
        {
            // given
            var startDateString = "2012-06-06";
            var endDateString = "happily ever after";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2000-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange!.Value.startDate.Should().Be(DateTime.Parse(startDateString));
            dateRange!.Value.endDate.Should().BeNull();
        }

        [Test]
        public void GetValidatedUsageStatsDateRange_returns_date_range()
        {
            // given
            var startDateString = "2012-06-06";
            var endDateString = "2021-06-06";
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(101, A<int?>._))
                .Returns(DateTime.Parse("2000-06-07"));

            // when
            var dateRange = activityService.GetValidatedUsageStatsDateRange(startDateString, endDateString, 101);

            // then
            dateRange!.Value.startDate.Should().Be(DateTime.Parse(startDateString));
            dateRange!.Value.endDate.Should().Be(DateTime.Parse(endDateString));
        }

        [Test]
        public void GetActivityStartDateForCentre_strips_time_from_database_value()
        {
            // given
            var dateWithTime = DateTime.Parse("2020/12/12 12:40:40");
            A.CallTo(() => activityDataService.GetStartOfActivityForCentre(A<int>._, A<int?>._))
                .Returns(dateWithTime);

            // when
            var result = activityService.GetActivityStartDateForCentre(1);

            // then
            result.Should().Be(dateWithTime.Date);
        }

        private void GivenActivityDataServiceReturnsDataInExampleSheet()
        {
            var activityResult = new List<ActivityLog>
            {
                new ActivityLog
                {
                    Completed = true,
                    Evaluated = false,
                    Registered = false,
                    LogDate = DateTime.Parse("2020-12-22"),
                    LogYear = 2020,
                    LogQuarter = 4,
                    LogMonth = 12,
                },
            };
            GivenActivityDataServiceReturnsSpecifiedResult(activityResult);
        }

        private void GivenActivityDataServiceReturnsSpecifiedResult(IEnumerable<ActivityLog> resultToReturn)
        {
            A.CallTo(
                    () => activityDataService.GetFilteredActivity(
                        A<int>._,
                        A<DateTime>._,
                        A<DateTime>._,
                        A<int?>._,
                        A<int?>._,
                        A<int?>._
                    )
                )
                .Returns(resultToReturn);
        }

        private void ValidatePeriodData(
            PeriodOfActivity periodData,
            string expectedDate,
            ReportInterval interval,
            int expectedRegistrations,
            int expectedCompletions,
            int expectedEvaluations
        )
        {
            periodData.Should().BeEquivalentTo(
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse(expectedDate), interval),
                    expectedRegistrations,
                    expectedCompletions,
                    expectedEvaluations
                )
            );
        }
    }
}
