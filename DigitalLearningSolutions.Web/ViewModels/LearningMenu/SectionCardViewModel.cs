﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class SectionCardViewModel
    {
        public string Title { get; }
        public int SectionId { get; }
        public string PercentComplete { get; }
        public int CustomisationId { get; }

        public SectionCardViewModel(CourseSection section, int customisationId, bool showPercentageCourseSetting)
        {
            Title = section.Title;
            SectionId = section.Id;
            PercentComplete = section.HasLearning && showPercentageCourseSetting
                ? $"{Convert.ToInt32(Math.Floor(section.PercentComplete))}% Complete"
                : "";
            CustomisationId = customisationId;
        }
    }
}
