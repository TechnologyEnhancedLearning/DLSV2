
namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;
    using System.Collections.Generic;
    using System.Data;
    public interface IRequestSupportTicketDataService
    {
        IEnumerable<RequestType> GetRequestTypes();

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
            return connection.Query<RequestType>(@$"SELECT TicketTypeId as ID,TypePrompt AS RequestTypes FROM [dbo].[TicketTypes]  order by TypePrompt");
        }
    }
}
