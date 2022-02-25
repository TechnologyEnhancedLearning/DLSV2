namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;

    public interface ISupportTicketDataService
    {
        int GetNumberOfUnarchivedTicketsForCentreId(int centreId);

        int GetNumberOfUnarchivedTicketsForAdminId(int adminId);
    }

    public class SupportTicketDataService : ISupportTicketDataService
    {
        private readonly IDbConnection connection;

        public SupportTicketDataService(IDbConnection connection)
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

        public int GetNumberOfUnarchivedTicketsForAdminId(int adminId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                    FROM Tickets AS t
                    WHERE t.ArchivedDate IS NULL AND t.TStatusID < 4 AND t.AdminUserID = @adminId",
                new { adminId }
            );
        }
    }
}
