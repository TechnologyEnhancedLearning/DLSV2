namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;

    public class AddNewCentreCourseData
    {
        public AddNewCentreCourseData()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public ApplicationDetails? Application { get; set; }
        public SetCourseDetailsViewModel? SetCourseDetailsModel { get; set; }
        public SetCourseOptionsViewModel? SetCourseOptionsModel { get; set; }
        public SetCourseContentViewModel? SetCourseContentModel { get; set; }
        public SetSectionContentViewModel? SetSectionContentModel { get; set; }

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
            SetSectionContentModel = null;
        }
    }
}
