namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AddNewCentreCourseData
    {
        public AddNewCentreCourseData()
        {
            SectionContentData = new List<SectionContentData>();
        }

        public ApplicationDetails? Application { get; set; }
        public CourseDetailsData? CourseDetailsData { get; set; }
        public CourseOptionsData? CourseOptionsData { get; set; }
        public CourseContentData? CourseContentData { get; set; }
        public List<SectionContentData>? SectionContentData { get; set; }

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

        public IEnumerable<CourseTutorialData> GetTutorialsFromSections()
        {
            var tutorials = new List<CourseTutorialData>();
            foreach (var section in SectionContentData!)
            {
                tutorials.AddRange(section.Tutorials);
            }

            return tutorials;
        }
    }
}
