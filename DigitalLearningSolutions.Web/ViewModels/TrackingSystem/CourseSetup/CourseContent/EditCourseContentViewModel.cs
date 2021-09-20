﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseContentViewModel
    {
        public EditCourseContentViewModel() {}

        public EditCourseContentViewModel(
            int customisationId,
            string courseName,
            IEnumerable<Section> sections
        )
        {
            CustomisationId = customisationId;
            CourseName = courseName;
            Sections = sections.Select(s => new EditCourseSectionViewModel(s));
        }

        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
        public IEnumerable<EditCourseSectionViewModel> Sections { get; set; }
    }
}
