namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using FluentAssertions;
    using NUnit.Framework;

    public class SelfAssessmentCardViewModelTests
    {
        [Test]
        public void Self_assessment_should_be_overdue_when_complete_by_date_is_in_the_past()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment(completeByDate: DateTime.Today - TimeSpan.FromDays(1));

            // When
            var selfAssessmentCardViewModel = new SelfAssessmentCardViewModel(selfAssessment, new ReturnPageQuery(1, "pageNumber=1"));

            // Then
            selfAssessmentCardViewModel.DateStyle().Should().Be("overdue");
        }

        [Test]
        public void Self_assessment_should__be_due_soon_when_complete_by_date_is_in_the_future()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment(completeByDate: DateTime.Today + TimeSpan.FromDays(1));


            // When
            var selfAssessmentCardViewModel = new SelfAssessmentCardViewModel(selfAssessment, new ReturnPageQuery(1, "pageNumber=1"));

            // Then
            selfAssessmentCardViewModel.DateStyle().Should().Be("due-soon");
        }

        [Test]
        public void Self_assessment_should_have_no_date_style_when_due_far_in_the_future()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment(completeByDate: DateTime.Today + TimeSpan.FromDays(100));

            // When
            var selfAssessmentCardViewModel = new SelfAssessmentCardViewModel(selfAssessment, new ReturnPageQuery(1, "pageNumber=1"));

            // Then
            selfAssessmentCardViewModel.DateStyle().Should().Be("");
        }
    }
}
