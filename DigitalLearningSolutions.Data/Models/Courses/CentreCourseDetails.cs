namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;

    public class CentreCourseDetails
    {
        public CentreCourseDetails() { }

        public CentreCourseDetails(
            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            Categories = categories;
            Topics = topics;
            Courses = courses;
        }

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> Courses { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Topics { get; set; }
    }
}
