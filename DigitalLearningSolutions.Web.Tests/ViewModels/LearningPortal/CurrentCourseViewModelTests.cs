namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using FluentAssertions;
    using NUnit.Framework;

    public class CurrentCourseViewModelTests
    {
        [Test]
        public void Current_course_should_be_overdue_when_complete_by_date_is_in_the_past()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today - TimeSpan.FromDays(1));

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));

            // Then
            currentCourseViewModel.DateStyle().Should().Be("overdue");
        }

        [Test]
        public void Current_course_should__be_due_soon_when_complete_by_date_is_in_the_future()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(1));


            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));


            // Then
            currentCourseViewModel.DateStyle().Should().Be("due-soon");
        }

        [Test]
        public void Current_course_should_have_no_date_style_when_due_far_in_the_future()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(100));

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));



            // Then
            currentCourseViewModel.DateStyle().Should().Be("");
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(hasDiagnostic: false);

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));


            // Then
            currentCourseViewModel.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_score_value()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(diagnosticScore: null);

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));


            // Then
            currentCourseViewModel.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_diagnostic_score_with_diagnostic_score_value_and_diagnostic_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));


            // Then
            currentCourseViewModel.HasDiagnosticScore().Should().BeTrue();
        }

        [Test]
        public void Current_course_should_not_have_passed_sections_without_learning_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(isAssessed: false);

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));

            // Then
            currentCourseViewModel.HasPassedSections().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_passed_sections_with_learning_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();

            // When
            var currentCourseViewModel = new CurrentCourseViewModel(currentCourse, new ReturnPageQuery("pageNumber=1"));


            // Then
            currentCourseViewModel.HasPassedSections().Should().BeTrue();
        }
    }
}
