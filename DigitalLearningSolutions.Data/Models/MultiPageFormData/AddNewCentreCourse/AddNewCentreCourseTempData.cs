namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AddNewCentreCourseTempData
    {
        public AddNewCentreCourseTempData()
        {
            SectionContentData = new List<SectionContentTempData>();
        }

        public string? CategoryFilter { get; set; }
        public string? TopicFilter { get; set; }
        public ApplicationDetails? Application { get; set; }
        public CourseDetailsTempData? CourseDetailsData { get; set; }
        public CourseOptionsTempData? CourseOptionsData { get; set; }
        public CourseContentTempData? CourseContentData { get; set; }
        public List<SectionContentTempData>? SectionContentData { get; set; }

        public void SetApplicationAndResetModels(ApplicationDetails application)
        {
            if (Application == application)
            {
                return;
            }

            Application = application;
            CourseDetailsData = null;
            CourseOptionsData = null;
            CourseContentData = null;
            SectionContentData = null;
        }

        public IEnumerable<CourseTutorialTempData> GetTutorialsFromSections()
        {
            var tutorials = new List<CourseTutorialTempData>();
            foreach (var section in SectionContentData!)
            {
                tutorials.AddRange(section.Tutorials);
            }

            return tutorials;
        }
    }
}
