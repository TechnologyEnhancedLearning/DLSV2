namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        private const string BaseSelectDelegateQuery =
            @"SELECT
                cd.CandidateID AS Id,
                cd.CandidateNumber,
                cd.AliasID,
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

        public DelegateUser? GetDelegateUserById(int id)
        {
            var user = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
                    WHERE cd.CandidateId = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public List<DelegateUser> GetDelegateUsersByUsername(string username)
        {
            var users = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
                    WHERE cd.Active = 1 AND
                         (cd.CandidateNumber = @username OR cd.EmailAddress = @username)",
                new { username }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetAllDelegateUsersByUsername(string username)
        {
            var users = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
                    WHERE cd.CandidateNumber = @username OR cd.EmailAddress = @username",
                new { username }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            var users = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
                    WHERE cd.EmailAddress = @emailAddress",
                new { emailAddress }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId)
        {
            var users = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
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
            int userId
        )
        {
            connection.Execute(
                @"UPDATE Users
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            PrimaryEmail = @primaryEmail,
                            ProfileImage = @profileImage,
                            ProfessionalRegistrationNumber = @professionalRegNumber,
                            HasBeenPromptedForPrn = @hasBeenPromptedForPrn,
                            JobGroupId = @jobGroupId
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

        public DelegateUser? GetDelegateUserByCandidateNumber(string candidateNumber, int centreId)
        {
            var user = connection.Query<DelegateUser>(
                @$"{BaseSelectDelegateQuery}
                    WHERE cd.CandidateNumber = @candidateNumber AND cd.CentreId = @centreId",
                new { candidateNumber, centreId }
            ).SingleOrDefault();

            return user;
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
    }
}
