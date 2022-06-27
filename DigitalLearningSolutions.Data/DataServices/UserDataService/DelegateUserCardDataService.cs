﻿namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        private const string DelegateEntitySelectQuery =
            @"SELECT
                da.ID,
                da.CandidateNumber,
                c.CentreName,
                da.CentreID,
                da.DateRegistered,
                c.Active AS CentreActive,
                COALESCE(ucd.Email, u.PrimaryEmail) as EmailAddress,
                u.FirstName,
                u.LastName,
                u.PasswordHash as Password,
                da.Approved,
                LTRIM(RTRIM(da.Answer1)) AS Answer1,
                LTRIM(RTRIM(da.Answer2)) AS Answer2,
                LTRIM(RTRIM(da.Answer3)) AS Answer3,
                LTRIM(RTRIM(da.Answer4)) AS Answer4,
                LTRIM(RTRIM(da.Answer5)) AS Answer5,
                LTRIM(RTRIM(da.Answer6)) AS Answer6,
                u.JobGroupId,
                jg.JobGroupName,
                da.SelfReg,
                da.ExternalReg,
                da.Active,
                u.HasBeenPromptedForPrn,
                u.ProfessionalRegistrationNumber,
                (SELECT ID
                    FROM AdminAccounts aa
                        WHERE aa.UserID = da.UserID
                            AND aa.CentreID = da.CentreID
                            AND aa.Active = 1
                ) AS AdminID
            FROM DelegateAccounts AS da
            INNER JOIN Centres AS c ON c.CentreID = da.CentreID
            INNER JOIN Users AS u ON u.ID = da.UserID
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        public DelegateUserCard? GetDelegateUserCardById(int id)
        {
            var user = connection.Query<DelegateUserCard>(
                @$"{DelegateEntitySelectQuery}
                        WHERE da.ID = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId)
        {
            return connection.Query<DelegateUserCard>(
                @$"{DelegateEntitySelectQuery}
                        WHERE da.CentreId = @centreId AND da.Approved = 1",
                new { centreId }
            ).ToList();
        }

        public List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId)
        {
            return connection.Query<DelegateUserCard>(
                @$"{DelegateEntitySelectQuery}
                        WHERE da.CentreId = @centreId
                        AND da.Approved = 1
                        AND da.Active = 1
                        AND NOT EXISTS (SELECT DelegateID FROM GroupDelegates WHERE DelegateID = da.ID
                                        AND GroupID = @groupId)",
                new
                {
                    centreId, groupId,
                }
            ).ToList();
        }
    }
}
