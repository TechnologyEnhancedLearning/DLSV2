namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class AddNewCentreCourseData
    {
        public AddNewCentreCourseData()
        {
            Id = Guid.NewGuid();
            SetSectionContentModels = new List<SetSectionContentViewModel>();
        }

        public Guid Id { get; set; }
        public ApplicationDetails? Application { get; set; }
        public SetCourseDetailsViewModel? SetCourseDetailsModel { get; set; }
        public EditCourseOptionsFormData? SetCourseOptionsModel { get; set; }
        public SetCourseContentViewModel? SetCourseContentModel { get; set; }
        public List<SetSectionContentViewModel> SetSectionContentModels { get; set; }

        public void SetApplicationAndResetModels(ApplicationDetails application)
        {
            if (Application == application)
            {
                return;
            }

            Application = application;
            SetCourseDetailsModel = null;
            SetCourseOptionsModel = null;
            SetCourseContentModel = null;
        }

        public IEnumerable<CourseTutorialViewModel> GetTutorialsFromSections()
        {
            var tutorials = new List<CourseTutorialViewModel>();
            foreach (var section in SetSectionContentModels)
            {
                tutorials.AddRange(section.Tutorials);
            }

            return tutorials;
        }
    }
}
