﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FluentAssertions;
    using NUnit.Framework;

    public class CurrentViewModelTests
    {
        private CurrentViewModel model;

        [SetUp]
        public void SetUp()
        {
            var headlineFigures = new HeadlineFigures
            {
                ActiveCentres = 339,
                Delegates = 329025,
                LearningTime = 649911,
                Completions = 162263
            };

            model = new CurrentViewModel(headlineFigures);
        }

        [TestCase(0, "Centres", "centres", 339)]
        [TestCase(1, "Learners", "learners", 329025)]
        [TestCase(2, "Learning Hours", "learning-hours", 649911)]
        [TestCase(3, "Courses Completed", "courses-completed", 162263)]
        public void Current_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedLabel,
            string expectedCssClassname,
            int expectedValue)
        {
            var course = model.CurrentCourses.ElementAt(index);
            course.Label.Should().Be(expectedLabel);
            course.CssClassname.Should().Be(expectedCssClassname);
            course.Value.Should().Be(expectedValue);
        }
    }
}
