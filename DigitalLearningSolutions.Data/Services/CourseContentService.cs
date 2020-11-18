namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using Microsoft.Extensions.Logging;

    public interface ICourseContentService
    {
        CourseContent? GetCourseContent(int customisationId);
    }

    public class CourseContentService : ICourseContentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseContentService> logger;

        public CourseContentService(IDbConnection connection, ILogger<CourseContentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public CourseContent? GetCourseContent(int customisationId)
        {
            return connection.QueryFirstOrDefault<CourseContent>(
                @"SELECT C.CustomisationID AS Id, C.CustomisationName, A.ApplicationName 
                      FROM Customisations as C
                      JOIN Applications AS A ON C.ApplicationID = A.ApplicationID
                      WHERE C.CustomisationID = @customisationId",
                new { customisationId }
            );
        }
    }
}
