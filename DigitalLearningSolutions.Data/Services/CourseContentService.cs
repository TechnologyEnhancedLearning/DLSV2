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
                @"SELECT Customisations.CustomisationID AS id,
                         Applications.ApplicationName,
	                     Customisations.CustomisationName,
	                     dbo.GetMinsForCustomisation(Customisations.CustomisationID) AS AverageDuration,
	                     Centres.CentreName,
	                     Centres.BannerText
                  FROM Applications
                  INNER JOIN Customisations ON Applications.ApplicationID = Customisations.ApplicationID
                  INNER JOIN Centres ON Customisations.CentreID = Centres.CentreID
                  WHERE Customisations.CustomisationID = @customisationId;",
                new { customisationId }
            );
        }
    }
}
