namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;

    public class CentreCourseDetails
    {
        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> Courses { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Topics { get; set; }
    }
}
