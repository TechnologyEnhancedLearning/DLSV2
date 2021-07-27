﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using DigitalLearningSolutions.Data.Models;

    public class CourseTutorialViewModel
    {
        public CourseTutorialViewModel(Tutorial tutorial)
        {
            TutorialId = tutorial.TutorialId;
            TutorialName = tutorial.TutorialName;
            LearningEnabled = tutorial.Status.HasValue && tutorial.Status.Value;
            DiagnosticEnabled = tutorial.DiagStatus.HasValue && tutorial.DiagStatus.Value;
        }

        public int TutorialId { get; set; }
        public string TutorialName { get; set; }
        public bool LearningEnabled { get; set; }
        public bool DiagnosticEnabled { get; set; }
    }
}
