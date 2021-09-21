namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateDataService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmail(int centreId, string email);

        void UpdateSupervisorDelegateRecordsCandidateId(IEnumerable<int> supervisorDelegateIds, int candidateId);

        void UpdateSupervisorDelegateRecordConfirmed(int supervisorDelegateId, DateTime? confirmed);
    }

    public class SupervisorDelegateDataService : ISupervisorDelegateDataService
    {
        private readonly IDbConnection connection;

        public SupervisorDelegateDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash)
        {
            return connection.QuerySingleOrDefault<SupervisorDelegate?>(
                @"SELECT
                        sd.ID,
                        sd.SupervisorAdminID,
                        sd.SupervisorEmail,
                        sd.CandidateID,
                        sd.DelegateEmail,
                        sd.Added,
                        sd.AddedByDelegate,
                        sd.NotificationSent,
                        sd.Confirmed,
                        sd.Removed,
                        au.CentreID
                    FROM SupervisorDelegates sd
                    INNER JOIN AdminUsers au ON sd.SupervisorAdminID = au.AdminID
                    WHERE sd.InviteHash = @inviteHash",
                new { inviteHash }
            );
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmail(int centreId, string email)
        {
            return connection.Query<SupervisorDelegate>(
                @"SELECT
                        sd.ID,
                        sd.SupervisorAdminID,
                        sd.SupervisorEmail,
                        sd.CandidateID,
                        sd.DelegateEmail,
                        sd.Added,
                        sd.AddedByDelegate,
                        sd.NotificationSent,
                        sd.Confirmed,
                        sd.Removed,
                        au.CentreID
                    FROM SupervisorDelegates sd
                    INNER JOIN AdminUsers au ON sd.SupervisorAdminID = au.AdminID
                    WHERE au.CentreID = @centreId
                      AND sd.DelegateEmail = @email
                      AND sd.CandidateID IS NULL
                      AND sd.Removed IS NULL",
                new { centreId, email }
            );
        }

        public void UpdateSupervisorDelegateRecordsCandidateId(IEnumerable<int> supervisorDelegateIds, int candidateId)
        {
            connection.Execute(
                @"UPDATE SupervisorDelegates
                    SET CandidateID = @candidateId,
                        Confirmed = NULL
                    WHERE ID IN @supervisorDelegateIds",
                new { supervisorDelegateIds, candidateId }
            );
        }

        public void UpdateSupervisorDelegateRecordConfirmed(int supervisorDelegateId, DateTime? confirmed)
        {
            connection.Execute(
                @"UPDATE SupervisorDelegates
                    SET Confirmed = @confirmed
                    WHERE ID = @supervisorDelegateId",
                new { supervisorDelegateId, confirmed }
            );
        }
    }
}
