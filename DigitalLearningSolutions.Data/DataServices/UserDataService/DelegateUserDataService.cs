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
                        ct.CentreName,
                        cd.CentreID,
                        cd.DateRegistered,
                        ct.Active AS CentreActive,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved,
                        cd.ProfileImage,
                        cd.Answer1,
                        cd.Answer2,
                        cd.Answer3,
                        cd.Answer4,
                        cd.Answer5,
                        cd.Answer6,
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
                        cd.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
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
                        cd.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
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
                        cd.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved,
                        cd.ResetPasswordID
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    WHERE cd.EmailAddress = @emailAddress",
                new { emailAddress }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID,
                        cd.CandidateNumber,
                        cd.FirstName,
                        cd.LastName,
                        cd.EmailAddress,
                        cd.DateRegistered,
                        cd.Answer1,
                        cd.Answer2,
                        cd.Answer3,
                        cd.Answer4,
                        cd.Answer5,
                        cd.Answer6,
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE (cd.Approved = 0)
                    AND (cd.Active = 1)
                    AND (cd.CentreID = @centreId)",
                new { centreId }
            ).ToList();

            return users;
        }

        public void UpdateDelegateUsers(string firstName, string surname, string email, byte[]? profileImage, int[] ids)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            EmailAddress = @email,
                            ProfileImage = @profileImage
                        WHERE CandidateID in @ids",
                new { firstName, surname, email, profileImage, ids }
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

        public bool? GetApprovedStatusFromCandidateNumber(string candidateNumber, int centreId)
        {
            return connection.Query<bool?>(
                @"SELECT Approved
                    FROM Candidates
                    WHERE CandidateNumber = @candidateNumber
                    AND CentreID = @centreId",
                new { candidateNumber, centreId }
            ).SingleOrDefault();
        }

        public bool? GetApprovedStatusFromAliasId(string aliasId, int centreId)
        {
            return connection.Query<bool?>(
                @"SELECT Approved
                    FROM Candidates
                    WHERE AliasID = @aliasId
                    AND CentreID = @centreId",
                new { aliasId, centreId }
            ).SingleOrDefault();
        }

        public int UpdateDelegateRecord(DelegateRecord record)
        {
            return connection.QuerySingle<int>(
                "uspUpdateCandidate_V7",
                record,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
