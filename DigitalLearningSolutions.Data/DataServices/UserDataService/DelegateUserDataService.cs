﻿namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        private const string BaseSelectDelegateUserQuery =
            @"SELECT
                cd.CandidateID AS Id,
                cd.CandidateNumber,
                ct.CentreName,
                cd.CentreID,
                cd.DateRegistered,
                ct.Active AS CentreActive,
                cd.EmailAddress,
                cd.FirstName,
                cd.LastName,
                cd.Password,
                cd.Active,
                cd.Approved,
                cd.ProfileImage,
                cd.Answer1,
                cd.Answer2,
                cd.Answer3,
                cd.Answer4,
                cd.Answer5,
                cd.Answer6,
                cd.JobGroupID,
                jg.JobGroupName,
                cd.HasBeenPromptedForPrn,
                cd.ProfessionalRegistrationNumber,
                cd.HasDismissedLhLoginWarning,
                cd.ResetPasswordID
            FROM Candidates AS cd
            INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID";

        private const string BaseSelectDelegateAccountQuery =
            @"SELECT
                da.ID,
                da.Active,
                da.CentreID,
                ce.CentreName,
                ce.Active AS CentreActive,
                da.DateRegistered,
                da.CandidateNumber,
                da.Answer1,
                da.Answer2,
                da.Answer3,
                da.Answer4,
                da.Answer5,
                da.Answer6,
                da.Approved,
                da.ExternalReg,
                da.SelfReg,
                da.OldPassword,
                da.UserID,
                da.CentreSpecificDetailsLastChecked,
                da.RegistrationConfirmationHash,
                da.RegistrationConfirmationHashCreationDateTime
            FROM DelegateAccounts AS da
            INNER JOIN Centres AS ce ON ce.CentreId = da.CentreId";

        private const string BaseDelegateEntitySelectQuery =
            @"SELECT
                da.ID,
                da.Active,
                da.CentreID,
                ce.CentreName,
                ce.Active AS CentreActive,
                da.DateRegistered,
                da.CandidateNumber,
                da.Answer1,
                da.Answer2,
                da.Answer3,
                da.Answer4,
                da.Answer5,
                da.Answer6,
                da.Approved,
                da.ExternalReg,
                da.SelfReg,
                da.OldPassword,
                da.UserID,
                da.CentreSpecificDetailsLastChecked,
                u.ID,
                u.PrimaryEmail,
                u.PasswordHash,
                u.FirstName,
                u.LastName,
                u.JobGroupID,
                jg.JobGroupName,
                u.ProfessionalRegistrationNumber,
                u.ProfileImage,
                u.Active,
                u.ResetPasswordID,
                u.TermsAgreed,
                u.FailedLoginCount,
                u.HasBeenPromptedForPrn,
                u.LearningHubAuthID,
                u.HasDismissedLhLoginWarning,
                u.EmailVerified,
                u.DetailsLastChecked,
                ucd.ID,
                ucd.UserID,
                ucd.CentreID,
                ucd.Email,
                ucd.EmailVerified,
                (SELECT ID
                    FROM AdminAccounts aa
                        WHERE aa.UserID = da.UserID
                            AND aa.CentreID = da.CentreID
                            AND aa.Active = 1
                ) AS AdminID
            FROM DelegateAccounts AS da
            INNER JOIN Centres AS ce ON ce.CentreId = da.CentreID
            INNER JOIN Users AS u ON u.ID = da.UserID
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = u.ID
            AND ucd.CentreId = da.CentreID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        public DelegateEntity? GetDelegateById(int id)
        {
            var sql = $@"{BaseDelegateEntitySelectQuery} WHERE da.ID = @id";

            return connection.Query<DelegateAccount, UserAccount, UserCentreDetails, int?, DelegateEntity>(
                sql,
                (delegateAccount, userAccount, userCentreDetails, adminId) => new DelegateEntity(
                    delegateAccount,
                    userAccount,
                    userCentreDetails,
                    adminId
                ),
                new { id },
                splitOn: "ID,ID,AdminID"
            ).SingleOrDefault();
        }

        public DelegateEntity? GetDelegateByCandidateNumber(string candidateNumber)
        {
            var sql = $@"{BaseDelegateEntitySelectQuery} WHERE da.CandidateNumber = @candidateNumber";

            return connection.Query<DelegateAccount, UserAccount, UserCentreDetails, DelegateEntity>(
                sql,
                (delegateAccount, userAccount, userCentreDetails) => new DelegateEntity(
                    delegateAccount,
                    userAccount,
                    userCentreDetails
                ),
                new { candidateNumber },
                splitOn: "ID,ID"
            ).SingleOrDefault();
        }
        
        public IEnumerable<DelegateEntity> GetUnapprovedDelegatesByCentreId(int centreId)
        {
            var sql =
                $@"{BaseDelegateEntitySelectQuery} WHERE da.Approved = 0 AND da.Active = 1 AND da.CentreID = @centreId";

            return connection.Query<DelegateAccount, UserAccount, UserCentreDetails, DelegateEntity>(
                sql,
                (delegateAccount, userAccount, userCentreDetails) => new DelegateEntity(
                    delegateAccount,
                    userAccount,
                    userCentreDetails
                ),
                new { centreId },
                splitOn: "ID,ID"
            );
        }

        [Obsolete("New code should use GetDelegateById instead")]
        public DelegateUser? GetDelegateUserById(int id)
        {
            var user = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateUserQuery}
                    WHERE cd.CandidateId = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        [Obsolete("New code should use GetUnapprovedDelegatesByCentreId instead")]
        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId)
        {
            var users = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateUserQuery}
                    WHERE (cd.Approved = 0)
                    AND (cd.Active = 1)
                    AND (cd.CentreID = @centreId)",
                new { centreId }
            ).ToList();

            return users;
        }

        public void UpdateUser(
            string firstName,
            string surname,
            string primaryEmail,
            byte[]? profileImage,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            int jobGroupId,
            DateTime detailsLastChecked,
            DateTime? emailVerified,
            int userId,
            bool isPrimaryEmailUpdated,
            bool changeMadeBySameUser = false
        )
        {
            connection.Execute(
                @"UPDATE Users
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            PrimaryEmail = @primaryEmail,
                            ProfileImage = (CASE WHEN @changeMadeBySameUser = 1 THEN @profileImage ELSE ProfileImage END),
                            ProfessionalRegistrationNumber = @professionalRegNumber,
                            HasBeenPromptedForPrn = @hasBeenPromptedForPrn,
                            JobGroupId = @jobGroupId,
                            DetailsLastChecked = (CASE WHEN @changeMadeBySameUser = 1 THEN @detailsLastChecked ELSE DetailsLastChecked END),
                            EmailVerified = @emailVerified
                        WHERE ID = @userId",
                new
                {
                    firstName,
                    surname,
                    primaryEmail,
                    profileImage,
                    userId,
                    professionalRegNumber,
                    hasBeenPromptedForPrn,
                    jobGroupId,
                    detailsLastChecked,
                    emailVerified,
                    changeMadeBySameUser,
                    isPrimaryEmailUpdated,
                }
            );
        }

        public void UpdateUserDetails(string firstName, string surname, string primaryEmail, int jobGroupId, int userId)
        {
            connection.Execute(
                @"UPDATE Users
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            PrimaryEmail = @primaryEmail,
                            JobGroupId = @jobGroupId
                        WHERE ID = @userId",
                new { firstName, surname, primaryEmail, jobGroupId, userId }
            );
        }

        public void ApproveDelegateUsers(params int[] ids)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET Approved = 1
                        WHERE CandidateID IN @ids",
                new { ids }
            );
        }

        public void RemoveDelegateAccount(int delegateId)
        {
            using var transaction = new TransactionScope();
            connection.Execute(
                @"
                DELETE FROM NotificationUsers
                    WHERE CandidateID = @delegateId

                DELETE FROM GroupDelegates
                    WHERE DelegateID = @delegateId

                DELETE FROM DelegateAccounts
                    WHERE ID = @delegateId",
                new { delegateId }
            );
            transaction.Complete();
        }

        public int GetNumberOfApprovedDelegatesAtCentre(int centreId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM Candidates WHERE Active = 1 AND Approved = 1 AND CentreID = @centreId",
                new { centreId }
            );
        }

        public void UpdateDelegateAccount(
            int delegateId,
            bool active,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET
                        Active = @active,
                        Answer1 = @answer1,
                        Answer2 = @answer2,
                        Answer3 = @answer3,
                        Answer4 = @answer4,
                        Answer5 = @answer5,
                        Answer6 = @answer6
                    WHERE CandidateID = @delegateId",
                new
                {
                    delegateId,
                    active,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                }
            );
        }

        public void DeactivateDelegateUser(int delegateId)
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET Active = 0
                    WHERE CandidateID = @delegateId",
                new { delegateId }
            );
        }

        public void ActivateDelegateUser(int delegateId)
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET Active = 1
                    WHERE CandidateID = @delegateId",
                new { delegateId }
            );
        }

        public void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status)
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET HasDismissedLhLoginWarning = @status
                    WHERE CandidateID = @delegateId",
                new { delegateId, status }
            );
        }

        public int? GetDelegateUserLearningHubAuthId(int delegateId)
        {
            return connection.Query<int?>(
                @"SELECT LearningHubAuthId
                    FROM Candidates
                    WHERE CandidateID = @delegateId",
                new { delegateId }
            ).Single();
        }

        public void SetDelegateUserLearningHubAuthId(int delegateId, int learningHubAuthId)
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET LearningHubAuthId = @learningHubAuthId
                    WHERE CandidateID = @delegateId",
                new { delegateId, learningHubAuthId }
            );
        }

        public void UpdateDelegateProfessionalRegistrationNumber(
            int delegateId,
            string? professionalRegistrationNumber,
            bool hasBeenPromptedForPrn
        )
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET
                        ProfessionalRegistrationNumber = @professionalRegistrationNumber,
                        HasBeenPromptedForPrn = @hasBeenPromptedForPrn
                    WHERE CandidateID = @delegateId",
                new { delegateId, professionalRegistrationNumber, hasBeenPromptedForPrn }
            );
        }

        public IEnumerable<DelegateAccount> GetDelegateAccountsByUserId(int userId)
        {
            return connection.Query<DelegateAccount>(
                @$"{BaseSelectDelegateAccountQuery} WHERE da.UserID = @userId",
                new { userId }
            );
        }

        public DelegateAccount? GetDelegateAccountById(int id)
        {
            return connection.QuerySingleOrDefault<DelegateAccount>(
                @$"{BaseSelectDelegateAccountQuery} WHERE da.ID = @id",
                new { id }
            );
        }

        public void SetRegistrationConfirmationHash(int userId, int centreId, string? hash)
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                    SET RegistrationConfirmationHash = @hash
                    WHERE UserID = @userId AND CentreID = @centreId",
                new { hash, userId, centreId }
            );
        }

        public void LinkDelegateAccountToNewUser(
            int currentUserIdForDelegateAccount,
            int newUserIdForDelegateAccount,
            int centreId
        )
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                    SET UserID = @newUserIdForDelegateAccount, RegistrationConfirmationHash = NULL
                    WHERE UserID = @currentUserIdForDelegateAccount AND CentreID = @centreId",
                new { currentUserIdForDelegateAccount, newUserIdForDelegateAccount, centreId }
            );
        }
    }
}
