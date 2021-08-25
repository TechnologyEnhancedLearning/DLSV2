namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface ICourseTopicsDataService
    {
        IEnumerable<Topic> GetCourseTopicsAvailableAtCentre(int centreId);
    }

    public class CourseTopicsDataService : ICourseTopicsDataService
    {
        private readonly IDbConnection connection;

        public CourseTopicsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Topic> GetCourseTopicsAvailableAtCentre(int centreId)
        {
            return connection.Query<Topic>(
                @"SELECT CourseTopicID, CourseTopic, Active
                    FROM CourseTopics
                    WHERE (CentreID = @CentreID OR CentreID = 0) AND (Active = 1)
                    ORDER BY CourseTopic",
                new { centreId }
            );
        }
    }
}
