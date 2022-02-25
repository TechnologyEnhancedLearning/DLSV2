namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class CourseStatisticsWithAdminFieldResponseCounts : CourseStatistics
    {
        public CourseStatisticsWithAdminFieldResponseCounts(){}

        public CourseStatisticsWithAdminFieldResponseCounts(
            CourseStatistics courseStatistics,
            IEnumerable<CourseAdminFieldWithResponseCounts> adminFieldsWithResponses
        )
        {
            AdminFieldsWithResponses = adminFieldsWithResponses;
            AllCentres = courseStatistics.AllCentres;
            DelegateCount = courseStatistics.DelegateCount;
            CompletedCount = courseStatistics.CompletedCount;
            AllAttempts = courseStatistics.AllAttempts;
            AttemptsPassed = courseStatistics.AttemptsPassed;
            HideInLearnerPortal = courseStatistics.HideInLearnerPortal;
            CategoryName = courseStatistics.CategoryName;
            CourseTopic = courseStatistics.CourseTopic;
            LearningMinutes = courseStatistics.LearningMinutes;
            IsAssessed = courseStatistics.IsAssessed;
            CustomisationId = courseStatistics.CustomisationId;
            CentreId = courseStatistics.CentreId;
            ApplicationId = courseStatistics.ApplicationId;
            Active = courseStatistics.Active;
            CustomisationName = courseStatistics.CustomisationName;
            ApplicationName = courseStatistics.ApplicationName;
        }

        public IEnumerable<CourseAdminFieldWithResponseCounts> AdminFieldsWithResponses { get; set; }

        public bool HasAdminFields => AdminFieldsWithResponses.Any();
    }
}
