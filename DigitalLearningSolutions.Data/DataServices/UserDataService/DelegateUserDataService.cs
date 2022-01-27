namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        public DelegateUser? GetDelegateUserById(int id)
        {
            var user = connection.Query<DelegateUser>(
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
                        cd.HasDismissedLhLoginWarning
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.CandidateId = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public List<DelegateUser> GetDelegateUsersByUsername(string username)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.Active = 1 AND
                         (cd.CandidateNumber = @username OR cd.EmailAddress = @username OR cd.AliasID = @username)",
                new { username }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetAllDelegateUsersByUsername(string username)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.CandidateNumber = @username OR cd.EmailAddress = @username OR cd.AliasID = @username",
                new { username }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
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
                        cd.ResetPasswordID
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.EmailAddress = @emailAddress",
                new { emailAddress }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE (cd.Approved = 0)
                    AND (cd.Active = 1)
                    AND (cd.CentreID = @centreId)",
                new { centreId }
            ).ToList();

            return users;
        }

        public void UpdateDelegateUsers(
            string firstName,
            string surname,
            string email,
            byte[]? profileImage,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            int[] ids
        )
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            EmailAddress = @email,
                            ProfileImage = @profileImage,
                            ProfessionalRegistrationNumber = @professionalRegNumber,
                            HasBeenPromptedForPrn = @hasBeenPromptedForPrn
                        WHERE CandidateID in @ids",
                new { firstName, surname, email, profileImage, ids, professionalRegNumber, hasBeenPromptedForPrn }
            );
        }

        public void UpdateDelegateAccountDetails(string firstName, string surname, string email, int[] ids)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            EmailAddress = @email
                        WHERE CandidateID in @ids",
                new { firstName, surname, email, ids }
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

        public void RemoveDelegateUser(int delegateId)
        {
            using var transaction = new TransactionScope();
            connection.Execute(
                @"
                DELETE FROM NotificationUsers
                    WHERE CandidateID = @delegateId

                DELETE FROM GroupDelegates
                    WHERE DelegateID = @delegateId

                DELETE FROM Candidates
                    WHERE CandidateID = @delegateId",
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

        public DelegateUser? GetDelegateUserByAliasId(string aliasId, int centreId)
        {
            var user = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.AliasID,
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.AliasID = @aliasId AND cd.CentreId = @centreId",
                new { aliasId, centreId }
            ).SingleOrDefault();

            return user;
        }

        public DelegateUser? GetDelegateUserByCandidateNumber(string candidateNumber, int centreId)
        {
            var user = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.AliasID,
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.CandidateNumber = @candidateNumber AND cd.CentreId = @centreId",
                new { candidateNumber, centreId }
            ).SingleOrDefault();

            return user;
        }

        public void UpdateDelegate(
            int delegateId,
            string firstName,
            string lastName,
            int jobGroupId,
            bool active,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            string? aliasId,
            string emailAddress
        )
        {
            connection.Execute(
                @"UPDATE Candidates
                    SET
                        FirstName = @firstName,
                        LastName = @lastName,
                        JobGroupID = @jobGroupId,
                        Active = @active,
                        Answer1 = @answer1,
                        Answer2 = @answer2,
                        Answer3 = @answer3,
                        Answer4 = @answer4,
                        Answer5 = @answer5,
                        Answer6 = @answer6,
                        AliasID = @aliasId,
                        EmailAddress = @emailAddress
                    WHERE CandidateID = @delegateId",
                new
                {
                    delegateId,
                    firstName,
                    lastName,
                    jobGroupId,
                    active,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                    aliasId,
                    emailAddress,
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

        public IEnumerable<DelegateUser> GetDelegateUsersByAliasId(string aliasId)
        {
            return connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.AliasID,
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
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.AliasID = @aliasId",
                new { aliasId }
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
