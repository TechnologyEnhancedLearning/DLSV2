namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;

    public interface ITicketDataService
    {
        public int GetNumberOfUnarchivedTicketsForCentreId(int centreId);
    }

    public class TicketDataService : ITicketDataService
    {
        private readonly IDbConnection connection;

        public TicketDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int GetNumberOfUnarchivedTicketsForCentreId(int centreId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                    FROM Tickets AS t
                    INNER JOIN AdminUsers AS au ON au.AdminID = t.AdminUserID
                    WHERE t.ArchivedDate IS NULL AND t.TStatusID < 4 AND au.CentreID = @centreId",
                new { centreId }
            );
        }
    }
}
