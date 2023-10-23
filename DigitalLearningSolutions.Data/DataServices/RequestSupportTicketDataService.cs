
namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;
    using System.Collections.Generic;
    using System.Data;
    public interface IRequestSupportTicketDataService
    {
        IEnumerable<RequestType> GetRequestTypes();
        string? GetUserCentreEmail(int userId, int centreId);

    }
    public class RequestSupportTicketDataService : IRequestSupportTicketDataService
    {
        private readonly IDbConnection connection;

        public RequestSupportTicketDataService(IDbConnection connection)
        {
            this.connection = connection;
        }
        public IEnumerable<RequestType> GetRequestTypes()
        {
            return connection.Query<RequestType>(@$"SELECT TicketTypeId as ID,TypePrompt AS RequestTypes,FreshdeskTicketType AS FreshdeskRequestTypes FROM [dbo].[TicketTypes]  order by TypePrompt");
        }
        public string? GetUserCentreEmail(int userId, int centreId)
        {
            return connection.QuerySingleOrDefault<string>(
                @"SELECT COALESCE(ucd.Email,u.PrimaryEmail) as Email  FROM UserCentreDetails ucd inner join users u on ucd.UserID=u.id
                    WHERE ucd.UserID=@userId and ucd.CentreID=@centreId",
                new { userId, centreId }
            );
        }

    }
}
