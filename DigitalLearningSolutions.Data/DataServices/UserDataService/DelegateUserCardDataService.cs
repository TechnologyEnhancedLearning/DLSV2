namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;
    using DocumentFormat.OpenXml.Drawing;

    public partial class UserDataService
    {
        private const string DelegateUserCardBlankRowSelectQuery =
            @"SELECT
                0 AS ID,
                NULL AS CandidateNumber,
                '' AS CentreName,
                0 AS CentreID,
                NULL AS DateRegistered,
                NULL AS RegistrationConfirmationHash,
                1 AS CentreActive,
                '' AS EmailAddress,
                '' AS FirstName,
                '' AS LastName,
                NULL AS Password,
                NULL AS EmailVerified,
                1 As Approved,
                '' AS Answer1,
                '' AS Answer2,
                '' AS Answer3,
                '' AS Answer4,
                '' AS Answer5,
                '' AS Answer6,
                NULL as JobGroupId,
                '' AS JobGroupName,
                0 AS SelfReg,
                0 AS ExternalReg,
                1 AS Active,
                0 AS HasBeenPromptedForPrn,
                '' AS ProfessionalRegistrationNumber,
                NULL AS AdminID";
        private const string DelegateUserCardSelectQuery =
            @"SELECT
                da.ID,
                da.CandidateNumber,
                c.CentreName,
                da.CentreID,
                da.DateRegistered,
                da.RegistrationConfirmationHash,
                c.Active AS CentreActive,
                COALESCE(ucd.Email, u.PrimaryEmail) AS EmailAddress,
                u.FirstName,
                u.LastName,
                u.PasswordHash AS Password,
                u.EmailVerified,
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
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.CentreID = da.CentreID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        private const string DelegateUserSelectQuery =
            @"SELECT
				da.ID,
				da.Active AS DelegateActive,
				da.CandidateNumber,
				c.CentreName,
				da.CentreID,
				da.DateRegistered,
                da.LastAccessed,
				da.RegistrationConfirmationHash,
				c.Active AS CentreActive,
				COALESCE(ucd.Email, u.PrimaryEmail) AS EmailAddress,
				u.Active AS UserActive,
				u.FirstName,
				u.LastName,
				u.PasswordHash AS Password,
				u.EmailVerified,
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
                da.Active,
				da.ExternalReg,
				u.HasBeenPromptedForPrn,
				u.ProfessionalRegistrationNumber,
                u.PrimaryEmail,
                ucd.Email,
				(SELECT ID
					FROM AdminAccounts aa
						WHERE aa.UserID = da.UserID
							AND aa.CentreID = da.CentreID
							AND aa.Active = 1
				) AS AdminID ";
        private const string DelegateUserExportSelectQuery =
            @"SELECT
                da.ID,
                da.CandidateNumber,
                c.CentreName,
                da.CentreID,
                da.DateRegistered,
                da.RegistrationConfirmationHash,
                c.Active AS CentreActive,
                COALESCE(ucd.Email, u.PrimaryEmail) AS EmailAddress,
                u.FirstName,
                u.LastName,
                u.PasswordHash AS Password,
                u.EmailVerified,
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
                ,u.PrimaryEmail
				,ucd.Email
				,da.Active as DelegateActive
            FROM DelegateAccounts AS da
            INNER JOIN Centres AS c ON c.CentreID = da.CentreID
            INNER JOIN Users AS u ON u.ID = da.UserID
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.CentreID = da.CentreID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";
        private const string DelegateUserFromTable = @" FROM DelegateAccounts AS da WITH (NOLOCK)
			INNER JOIN Centres AS c WITH (NOLOCK) ON c.CentreID = da.CentreID
			INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
			LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.CentreID = da.CentreID
			INNER JOIN JobGroups AS jg WITH (NOLOCK) ON jg.JobGroupID = u.JobGroupID ";
        private string DelegatewhereConditon = $@" Where ((CentreID = @centreId) OR (@centreId= 0))
                            AND ( FirstName + ' ' + LastName + ' ' + PrimaryEmail + ' ' + COALESCE(Email, '') + ' ' + COALESCE(CandidateNumber, '') LIKE N'%' + @searchString + N'%')
					        AND ((@isActive = 'Any') OR (@isActive = 'true' AND DelegateActive = 1) OR (@isActive = 'false' AND DelegateActive = 0))
					        AND ((@isPasswordSet = 'Any') OR (@isPasswordSet = 'true' AND (Password <>'')) OR (@isPasswordSet = 'false' AND (Password ='')))
					        AND ((@isAdmin = 'Any') OR (@isAdmin = 'true' AND (AdminID is not null)) OR (@isAdmin = 'false' AND (AdminID is null)))
					        AND ((@isUnclaimed = 'Any') OR (@isUnclaimed = 'true' AND (RegistrationConfirmationHash is not null)) OR (@isUnclaimed = 'false' AND (RegistrationConfirmationHash is null)))
					        AND ((@isEmailVerified = 'Any') OR (@isEmailVerified = 'true' AND EmailVerified IS NOT NULL) OR (@isEmailVerified = 'false' AND EmailVerified IS NULL))

					        AND ((@registrationType = 'Any') OR (@registrationType = 'SelfRegistered' AND SelfReg = 1 AND ExternalReg = 0) OR 
					        (@registrationType = 'SelfRegisteredExternal' AND SelfReg = 1 AND ExternalReg = 1) OR 
					        (@registrationType = 'RegisteredByCentre' AND SelfReg = 0 AND (ExternalReg = 0 OR ExternalReg = 1)))

                            AND ((@jobGroupId = 0) OR (JobGroupID = @jobGroupId ))

                            AND ((@answer1 = 'Any') OR (@answer1 = 'No option selected' AND (Answer1 IS NULL)) OR(Answer1 = @answer1))
                            AND ((@answer2 = 'Any') OR (@answer2 = 'No option selected' AND (Answer2 IS NULL)) OR (Answer2 = @answer2))
                            AND ((@answer3 = 'Any') OR (@answer3 = 'No option selected' AND (Answer3 IS NULL)) OR (Answer3 = @answer3))
                            AND ((@answer4 = 'Any') OR (@answer4 = 'No option selected' AND (Answer4 IS NULL)) OR (Answer4 = @answer4))
                            AND ((@answer5 = 'Any') OR (@answer5 = 'No option selected' AND (Answer5 IS NULL)) OR (Answer5 = @answer5))
                            AND ((@answer6 = 'Any') OR (@answer6 = 'No option selected' AND (Answer6 IS NULL)) OR (Answer6 = @answer6))

                            AND Approved = 1

                            AND EmailAddress LIKE '%_@_%'";
        public DelegateUserCard? GetDelegateUserCardById(int id)
        {
            var user = connection.Query<DelegateUserCard>(
                @$"{DelegateUserCardSelectQuery}
                        WHERE da.ID = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId)
        {
            if (centreId > 0)
            {
                return connection.Query<DelegateUserCard>(
                @$"{DelegateUserCardSelectQuery}
                        WHERE da.CentreId = @centreId AND da.Approved = 1",
            new { centreId }
            ).ToList();
            }
            else
            {
                return connection.Query<DelegateUserCard>(
                                @$"{DelegateUserCardBlankRowSelectQuery}"
                            ).ToList();
            }
        }
        public int GetCountDelegateUserCardsForExportByCentreId(String searchString, string sortBy, string sortDirection, int centreId,
                                     string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                     int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }

            if (groupId.HasValue)
            {
                var groupDelegatesForCentre = $@"SELECT DelegateID FROM GroupDelegates WHERE GroupID in (
											SELECT GroupID FROM Groups WHERE CentreID = @centreId AND RemovedDate IS NULL
											)";
                DelegatewhereConditon += "AND D.ID IN ( " + groupDelegatesForCentre + " AND GroupID = @groupId )";
            }


            var delegateCountQuery = @$"SELECT  COUNT(*) AS Matches FROM ( " + DelegateUserExportSelectQuery + " ) D " + DelegatewhereConditon;

            int ResultCount = connection.ExecuteScalar<int>(
                delegateCountQuery,
                new
                {
                    searchString,
                    sortBy,
                    sortDirection,
                    centreId,
                    isActive,
                    isPasswordSet,
                    isAdmin,
                    isUnclaimed,
                    isEmailVerified,
                    registrationType,
                    jobGroupId,
                    groupId,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6
                },
                commandTimeout: 3000
            );
            return ResultCount;
        }

        public List<DelegateUserCard> GetDelegateUserCardsForExportByCentreId(String searchString, string sortBy, string sortDirection, int centreId,
                                     string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                     int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6, int exportQueryRowLimit, int currentRun)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            if (groupId.HasValue)
            {
                var groupDelegatesForCentre = $@"SELECT DelegateID FROM GroupDelegates WHERE GroupID in (
											SELECT GroupID FROM Groups WHERE CentreID = @centreId AND RemovedDate IS NULL
											)";
                DelegatewhereConditon += "AND D.ID IN ( " + groupDelegatesForCentre + " AND GroupID = @groupId )";
            }

            string orderBy;
            string sortOrder;
            if (sortDirection == "Ascending")
                sortOrder = " ASC ";
            else
                sortOrder = " DESC ";

            if (sortBy == "SearchableName")
                orderBy = " ORDER BY LTRIM(LastName) " + sortOrder + ", LTRIM(FirstName) ";
            else
                orderBy = " ORDER BY DateRegistered " + sortOrder;

            orderBy += " OFFSET @exportQueryRowLimit * (@currentRun - 1) ROWS  FETCH NEXT @exportQueryRowLimit ROWS ONLY";
            var mainSql = "SELECT * FROM ( " + DelegateUserExportSelectQuery + " ) D " + DelegatewhereConditon + orderBy;

            IEnumerable<DelegateUserCard> delegateUserCard = connection.Query<DelegateUserCard>(
                mainSql,
                new
                {
                    searchString,
                    sortBy,
                    sortDirection,
                    centreId,
                    isActive,
                    isPasswordSet,
                    isAdmin,
                    isUnclaimed,
                    isEmailVerified,
                    registrationType,
                    jobGroupId,
                    groupId,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                    exportQueryRowLimit,
                    currentRun
                },
                commandTimeout: 3000
            );
            return (delegateUserCard).ToList();
        }
        public (IEnumerable<DelegateUserCard>, int) GetDelegateUserCards(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }


            var groupDelegatesForCentre = $@"SELECT DelegateID FROM GroupDelegates WHERE GroupID in (
											SELECT GroupID FROM Groups WHERE CentreID = @centreId AND RemovedDate IS NULL
											)";
            if (groupId.HasValue)
                DelegatewhereConditon += "AND D.ID IN ( " + groupDelegatesForCentre + " AND GroupID = @groupId )";


            string orderBy;

            if (sortDirection == "Ascending")
                sortDirection = " ASC ";
            else
                sortDirection = " DESC ";

            if (sortBy == "SearchableName")
                orderBy = " ORDER BY LTRIM(LastName) " + sortDirection + ", LTRIM(FirstName) ";
            else
                orderBy = " ORDER BY DateRegistered " + sortDirection;

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var mainSql = "SELECT * FROM ( " + DelegateUserSelectQuery + DelegateUserFromTable + " ) D " + DelegatewhereConditon + orderBy;

            IEnumerable<DelegateUserCard> delegateUserCard = connection.Query<DelegateUserCard>(
                mainSql,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    centreId,
                    isActive,
                    isPasswordSet,
                    isAdmin,
                    isUnclaimed,
                    isEmailVerified,
                    registrationType,
                    jobGroupId,
                    groupId,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6
                },
                commandTimeout: 3000
            );

            var delegateCountQuery = @$"SELECT  COUNT(*) AS Matches FROM ( " + DelegateUserSelectQuery + DelegateUserFromTable + " ) D " + DelegatewhereConditon;

            int ResultCount = connection.ExecuteScalar<int>(
                delegateCountQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    centreId,
                    isActive,
                    isPasswordSet,
                    isAdmin,
                    isUnclaimed,
                    isEmailVerified,
                    registrationType,
                    jobGroupId,
                    groupId,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                },
                commandTimeout: 3000
            );
            return (delegateUserCard, ResultCount);
        }

        public List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId)
        {
            return connection.Query<DelegateUserCard>(
                @$"{DelegateUserCardSelectQuery}
                        WHERE da.CentreId = @centreId
                        AND da.Approved = 1
                        AND da.Active = 1
                        AND (u.PrimaryEmail like '%_@_%' OR ucd.Email IS NOT NULL)
                        AND NOT EXISTS (SELECT DelegateID FROM GroupDelegates WHERE DelegateID = da.ID
                                        AND GroupID = @groupId)",
                new
                {
                    centreId,
                    groupId,
                }
            ).ToList();
        }
    }
}
