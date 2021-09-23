namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                    LogMonth = 12
                }
            };
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
                .Returns(expectedActivityResult);
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
        public void GetFilterOptions_returns_courses_in_alphabetical_order()
        {
            // Given
            var expectedCourses = new[] { (2, "A Course"), (1, "B Course") };
            
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
        public void GetFilterNames_returns_expected_job_group_name_with_non_null_job_groud_id_filter()
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
                new Category { CourseCategoryID = 2, CategoryName = "Category 2" }
            };
            var courses = new List<Course>
            {
                new Course { CustomisationId = 1, ApplicationName = "B Course" },
                new Course { CustomisationId = 2, ApplicationName = "A Course" }
            };
            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(jobGroups);
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId))
                .Returns(categories);
            A.CallTo(() => courseDataService.GetCentrallyManagedAndCentreCourses(centreId, categoryId))
                .Returns(courses);
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
