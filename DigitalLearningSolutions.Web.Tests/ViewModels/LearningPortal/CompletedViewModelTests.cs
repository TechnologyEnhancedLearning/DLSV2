namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CompletedViewModelTests
    {
        private readonly IConfiguration config = A.Fake<IConfiguration>();
        private CompletedViewModel model;
        private CompletedCourse[] completedCourses;

        [SetUp]
        public void SetUp()
        {
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");

            completedCourses = new[]
            {
                new CompletedCourse
                {
                    CustomisationID = 71,
                    CourseName = "B: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2010, 1, 31),
                    Completed = new DateTime(2010, 2, 22),
                    Evaluated = new DateTime(2010, 3, 22),
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6,
                },
                new CompletedCourse
                {
                    CustomisationID = 72,
                    CourseName = "C: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 2, 1),
                    Completed = new DateTime(2011, 2, 22),
                    Evaluated = new DateTime(2011, 3, 22),
                    DiagnosticScore = 0,
                    Passes = 14,
                    Sections = 16,
                },
                new CompletedCourse
                {
                    CustomisationID = 73,
                    CourseName = "A: Course",
                    HasDiagnostic = false,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2001, 1, 22),
                    Completed = new DateTime(2011, 2, 23),
                    Evaluated = new DateTime(2011, 2, 23),
                    DiagnosticScore = 0,
                    Passes = 0,
                    Sections = 6,
                },
            };

            model = new CompletedViewModel(
                completedCourses,
                config,
                null
            );
        }

        [TestCase(
            0,
            71,
            "B: Course",
            true,
            true,
            true,
            "2010-1-31",
            "2010-2-22",
            "2010-3-22",
            123,
            4,
            6,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=71&lp=1"
        )]
        [TestCase(
            1,
            72,
            "C: Course",
            true,
            true,
            false,
            "2010-2-1",
            "2011-2-22",
            "2011-3-22",
            0,
            14,
            16,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=72&lp=1"
        )]
        [TestCase(
            2,
            73,
            "A: Course",
            false,
            true,
            true,
            "2001-1-22",
            "2011-2-23",
            "2011-2-23",
            0,
            0,
            06,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=73&lp=1"
        )]
        public void Completed_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            int expectedId,
            string expectedName,
            bool expectedDiagnostic,
            bool expectedLearning,
            bool expectedAssessmentAndCertification,
            DateTime expectedStart,
            DateTime expectedCompleted,
            DateTime expectedEvaluated,
            int expectedDiagnosticScore,
            int expectedPasses,
            int expectedSections,
            string expectedLaunchUrl)
        {
            var course = model.CompletedCourses.ElementAt(index);
            course.Id.Should().Be(expectedId);
            course.Name.Should().Be(expectedName);
            course.HasDiagnosticAssessment.Should().Be(expectedDiagnostic);
            course.HasLearningContent.Should().Be(expectedLearning);
            course.HasLearningAssessmentAndCertification.Should().Be(expectedAssessmentAndCertification);
            course.StartedDate.Should().Be(expectedStart);
            course.CompletedDate.Should().Be(expectedCompleted);
            course.EvaluatedDate.Should().Be(expectedEvaluated);
            course.DiagnosticScore.Should().Be(expectedDiagnosticScore);
            course.PassedSections.Should().Be(expectedPasses);
            course.Sections.Should().Be(expectedSections);
        }
    }
}
