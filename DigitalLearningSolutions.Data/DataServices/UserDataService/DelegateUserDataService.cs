namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
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
                        cd.HasBeenPromptedForPrn,
                        cd.ProfessionalRegistrationNumber,
                        jg.JobGroupName
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
                        jg.JobGroupName
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
            int[] ids)
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

        public int UpdateDelegateRecord(DelegateRecord record)
        {
            return connection.QuerySingle<int>(
                "uspUpdateCandidate_V7",
                new
                {
                    record.CentreId,
                    DelegateID = record.CandidateNumber,
                    record.FirstName,
                    record.LastName,
                    record.JobGroupId,
                    record.Active,
                    record.Answer1,
                    record.Answer2,
                    record.Answer3,
                    record.Answer4,
                    record.Answer5,
                    record.Answer6,
                    record.AliasId,
                    record.Approved,
                    record.Email,
                },
                commandType: CommandType.StoredProcedure
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
    }
}
