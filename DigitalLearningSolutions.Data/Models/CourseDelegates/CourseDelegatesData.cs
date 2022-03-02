namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class CourseDelegatesData
    {
        public CourseDelegatesData(
            int? customisationId,
            IEnumerable<Course> courses,
            IEnumerable<CourseDelegate> delegates,
            IEnumerable<CustomPrompt> courseAdminFields
        )
        {
            CustomisationId = customisationId;
            Courses = courses;
            Delegates = delegates;
            CourseAdminFields = courseAdminFields;
        }

        public int? CustomisationId { get; set; }

        public IEnumerable<Course> Courses { get; set; }

        public IEnumerable<CourseDelegate> Delegates { get; set; }

        public IEnumerable<CustomPrompt> CourseAdminFields { get; set; }
    }
}
