﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CourseSectionViewModel
    {
        public CourseSectionViewModel(Section section, int customisationId, bool postLearningAssessment)
        {
            CustomisationId = customisationId;
            SectionId = section.SectionId;
            SectionName = section.SectionName;
            Tutorials = section.Tutorials.Select(t => new CourseTutorialViewModel(t));
            PostLearningAssessment = postLearningAssessment;
        }

        public int CustomisationId { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<CourseTutorialViewModel> Tutorials { get; set; }
        public bool PostLearningAssessment { get; set; }
    }
}
