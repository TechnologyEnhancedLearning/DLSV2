namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        private const string DelegateUserSelectQuery = @"SELECT cd.CandidateID AS Id,
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
                                                        cd.Answer1,
                                                        cd.Answer2,
                                                        cd.Answer3,
                                                        cd.Answer4,
                                                        cd.Answer5,
                                                        cd.Answer6,
                                                        cd.JobGroupId,
                                                        jg.JobGroupName,
                                                        cd.SelfReg,
                                                        cd.ExternalReg,
                                                        cd.Active,
                                                        cd.HasBeenPromptedForPrn,
                                                        cd.ProfessionalRegistrationNumber,
                                                        (SELECT AdminID
                                                            FROM AdminUsers au
                                                                WHERE (au.Email = cd.EmailAddress
                                                                OR au.Email = cd.AliasID)
                                                            AND au.Password = cd.Password
                                                                AND au.CentreID = cd.CentreID
                                                                AND au.Email != ''
                                                        ) AS AdminID,
                                                        cd.AliasID
                                                        FROM Candidates AS cd
                                                          INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                                                          INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID";

        public DelegateUserCard? GetDelegateUserCardById(int id)
        {
            var user = connection.Query<DelegateUserCard>(
                @$"{DelegateUserSelectQuery}
                        WHERE cd.CandidateId = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId)
        {
            return connection.Query<DelegateUserCard>(
                @$"{DelegateUserSelectQuery}
                        WHERE cd.CentreId = @centreId AND cd.Approved = 1",
                new { centreId }
            ).ToList();
        }

        public List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId)
        {
            return connection.Query<DelegateUserCard>(
                @$"{DelegateUserSelectQuery}
                        WHERE cd.CentreId = @centreId
                        AND cd.Approved = 1
                        AND cd.Active = 1
                        AND NOT EXISTS (SELECT DelegateID FROM GroupDelegates WHERE DelegateID = cd.CandidateID
                                        AND GroupID = @groupId)",
                new
                {
                    centreId, groupId,
                }
            ).ToList();
        }
    }
}
