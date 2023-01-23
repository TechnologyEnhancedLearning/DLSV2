namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using Microsoft.Extensions.Logging;

    public interface ISupervisorDelegateDataService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string> emails
        );

        void UpdateSupervisorDelegateRecordsCandidateId(IEnumerable<int> supervisorDelegateIds, int candidateId);
    }

    public class SupervisorDelegateDataService : ISupervisorDelegateDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorDelegateDataService> logger;

        public SupervisorDelegateDataService(IDbConnection connection, ILogger<SupervisorDelegateDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash)
        {
            try
            {
                return connection.QuerySingleOrDefault<SupervisorDelegate?>(
                    @"SELECT
                        sd.ID,
                        sd.SupervisorAdminID,
                        sd.SupervisorEmail,
                        sd.DelegateUserID,
                        sd.DelegateEmail,
                        sd.Added,
                        sd.AddedByDelegate,
                        sd.NotificationSent,
                        sd.Removed,
                        au.CentreID
                    FROM SupervisorDelegates sd
                    INNER JOIN AdminUsers au ON sd.SupervisorAdminID = au.AdminID
                    WHERE sd.InviteHash = @inviteHash",
                    new { inviteHash }
                );
            }
            catch (InvalidOperationException)
            {
                logger.LogError($"Multiple SupervisorDelegate records found with InviteHash {inviteHash}");
                return null;
            }
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string> emails
        )
        {
            return connection.Query<SupervisorDelegate>(
                @"SELECT
                        sd.ID,
                        sd.SupervisorAdminID,
                        sd.SupervisorEmail,
                        sd.DelegateUserID,
                        sd.DelegateEmail,
                        sd.Added,
                        sd.AddedByDelegate,
                        sd.NotificationSent,
                        sd.Removed,
                        au.CentreID
                    FROM SupervisorDelegates sd
                    INNER JOIN AdminUsers au ON sd.SupervisorAdminID = au.AdminID
                    WHERE au.CentreID = @centreId
                      AND sd.DelegateEmail IN @emails
                      AND sd.DelegateUserID IS NULL
                      AND sd.Removed IS NULL",
                new { centreId, emails }
            );
        }

        // TODO: HEEDLS-1014 - Change CandidateID to UserID
        public void UpdateSupervisorDelegateRecordsCandidateId(IEnumerable<int> supervisorDelegateIds, int delegateUserId)
        {
            connection.Execute(
                @"UPDATE SupervisorDelegates
                    SET DelegateUserID = @delegateUserId
                    WHERE ID IN @supervisorDelegateIds",
                new { supervisorDelegateIds, delegateUserId }
            );
        }
    }
}
