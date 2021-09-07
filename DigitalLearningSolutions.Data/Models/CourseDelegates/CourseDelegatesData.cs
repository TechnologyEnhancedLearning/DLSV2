namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseDelegatesData
    {
        public CourseDelegatesData(
            int? customisationId,
            IEnumerable<Course> courses,
            IEnumerable<CourseDelegate> delegates
        )
        {
            CustomisationId = customisationId;
            Courses = courses;
            Delegates = delegates;
        }

        public int? CustomisationId { get; set; }

        public IEnumerable<Course> Courses { get; set; }

        public IEnumerable<CourseDelegate> Delegates { get; set; }
    }
}
