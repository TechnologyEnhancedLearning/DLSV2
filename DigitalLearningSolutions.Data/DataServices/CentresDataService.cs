namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Transactions;

    public interface ICentresDataService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
        IEnumerable<(int, string)> GetCentresForDelegateSelfRegistrationAlphabetical();
        Centre? GetCentreDetailsById(int centreId);
        (IEnumerable<CentreEntity>, int) GetAllCentreSummariesForSuperAdmin(string search, int offset, int rows, int region,
          int centreType,
          int contractType,
          string centreStatus);
        IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre();

        CentreSummaryForContactDisplay GetCentreSummaryForContactDisplay(int centreId);

        CentreSummaryForRoleLimits GetRoleLimitsForCentre(int centreId);

        void UpdateCentreRoleLimits(
            int centreId,
            int? roleLimitCmsAdministrators,
            int? roleLimitCmsManagers,
            int? roleLimitCcLicences,
            int? roleLimitCustomCourses,
            int? roleLimitTrainers
        );

        void UpdateCentreManagerDetails(
            int centreId,
            string firstName,
            string lastName,
            string email,
            string? telephone
        );

        void UpdateCentreWebsiteDetails(
            int centreId,
            string postcode,
            double latitude,
            double longitude,
            string? telephone,
            string email,
            string? openingHours,
            string? webAddress,
            string? organisationsCovered,
            string? trainingVenues,
            string? otherInformation
        );

        void UpdateCentreDetails(
            int centreId,
            string? notifyEmail,
            string bannerText,
            byte[]? centreSignature,
            byte[]? centreLogo
        );

        public int AddCentreForSuperAdmin(
            string centreName,
            string? contactFirstName,
            string? contactLastName,
            string? contactEmail,
            string? contactPhone,
            int? centreTypeId,
            int? regionId,
            string? registrationEmail,
            string? ipPrefix,
            bool showOnMap,
            bool AddITSPcourses
        );

        public void UpdateCentreDetailsForSuperAdmin(
            int centreId,
            string centreName,
            int centreTypeId,
            int regionId,
            string? centreEmail,
            string? ipPrefix,
            bool showOnMap
        );

        (string firstName, string lastName, string email) GetCentreManagerDetails(int centreId);
        string[] GetCentreIpPrefixes(int centreId);
        (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId);
        void SetCentreAutoRegistered(int centreId);
        IEnumerable<CentreRanking> GetCentreRanks(DateTime dateSince, int? regionId, int resultsCount, int centreId);
        IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap();
        IEnumerable<(int, string)> GetAllCentres(bool? activeOnly = false);
        IEnumerable<(int, string)> GetCentreTypes();
        Centre? GetFullCentreDetailsById(int centreId);
        void DeactivateCentre(int centreId);
        void ReactivateCentre(int centreId);
        Centre? GetCentreManagerDetailsByCentreId(int centreId);
        ContractInfo? GetContractInfo(int centreId);
        bool UpdateContractTypeandCenter(
       int centreId,
       int contractTypeID,
       long delegateUploadSpace,
      long serverSpaceBytesInc,
      DateTime? contractReviewDate
   );
        public int ResultCount(string search, int region, int centreType, int contractType,
            string centreStatus);

        public IEnumerable<CentresExport> GetAllCentresForSuperAdminExport(string search, int region,
          int centreType, int contractType, string centreStatus, int exportQueryRowLimit, int currentRun);
    }

    public class CentresDataService : ICentresDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CentresDataService> logger;

        public CentresDataService(IDbConnection connection, ILogger<CentresDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public string? GetBannerText(int centreId)
        {
            return connection.QueryFirstOrDefault<string?>(
                @"SELECT BannerText
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }

        public string? GetCentreName(int centreId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT CentreName
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
            if (name == null)
            {
                logger.LogWarning
                (
                    $"No centre found for centre id {centreId}"
                );
            }

            return name;
        }

        public IEnumerable<(int, string)> GetCentresForDelegateSelfRegistrationAlphabetical()
        {
            var centres = connection.Query<(int, string)>
            (
                @"SELECT CentreID, CentreName
                        FROM Centres
                        WHERE Active = 1
                        AND kbSelfRegister = 1
                        ORDER BY CentreName"
            );
            return centres;
        }

        public Centre? GetFullCentreDetailsById(int centreId)
        {
            var centre = connection.QueryFirstOrDefault<Centre>(
                @"SELECT c.CentreID,
                            c.CentreName,
                            r.RegionName,
                            c.ContactForename,
                            c.ContactSurname,
                            c.ContactEmail,
                            c.ContactTelephone,
                            c.pwEmail AS CentreEmail,
                            c.ShowOnMap,
                            c.CMSAdministrators AS CmsAdministratorSpots,
                            c.CMSManagers AS CmsManagerSpots,
                            c.CCLicences AS CcLicenceSpots,
                            c.Trainers AS TrainerSpots,
                            c.IPPrefix,
                            ct.ContractType,
                            c.CustomCourses,
                            c.ServerSpaceBytes,
                            cty.CentreType,
                            c.CandidateByteLimit,
                            c.ContractReviewDate
                        FROM Centres AS c
                        INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                        INNER JOIN ContractTypes AS ct ON ct.ContractTypeID = c.ContractTypeId
                        INNER JOIN CentreTypes AS cty ON cty.CentreTypeId = c.CentreTypeId
                        WHERE CentreID = @centreId",
                new { centreId }
            );

            if (centre == null)
            {
                logger.LogWarning($"No centre found for centre id {centreId}");
                return null;
            }

            if (centre.CentreLogo?.Length < 10)
            {
                centre.CentreLogo = null;
            }

            return centre;
        }

        public Centre? GetCentreDetailsById(int centreId)
        {
            var centre = connection.QueryFirstOrDefault<Centre>(
                @"SELECT c.CentreID,
                            c.CentreName,
                            c.Active,
                            c.RegionID,
                            r.RegionName,
                            c.NotifyEmail,
                            c.BannerText,
                            c.SignatureImage,
                            c.CentreLogo,
                            c.ContactForename,
                            c.ContactSurname,
                            c.ContactEmail,
                            c.ContactTelephone,
                            c.pwTelephone AS CentreTelephone,
                            c.pwEmail AS CentreEmail,
                            c.pwPostCode AS CentrePostcode,
                            c.ShowOnMap,
                            c.Long AS Longitude,
                            c.Lat AS Latitude,
                            c.pwHours AS OpeningHours,
                            c.pwWebURL AS CentreWebAddress,
                            c.pwTrustsCovered AS OrganisationsCovered,
                            c.pwTrainingLocations AS TrainingVenues,
                            c.pwGeneralInfo AS OtherInformation,
                            c.CMSAdministrators AS CmsAdministratorSpots,
                            c.CMSManagers AS CmsManagerSpots,
                            c.CCLicences AS CcLicenceSpots,
                            c.Trainers AS TrainerSpots,
                            c.IPPrefix,
                            ct.ContractType,
                            c.CustomCourses,
                            c.ServerSpaceUsed,
                            c.ServerSpaceBytes,
                            c.CentreTypeID,
                            ctp.CentreType,
                            c.pwEmail as RegistrationEmail
                        FROM Centres AS c
                        INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                        INNER JOIN ContractTypes AS ct ON ct.ContractTypeID = c.ContractTypeId
                        INNER JOIN CentreTypes AS ctp ON ctp.CentreTypeID = c.CentreTypeID
                        WHERE CentreID = @centreId",
                new { centreId }
            );

            if (centre == null)
            {
                logger.LogWarning($"No centre found for centre id {centreId}");
                return null;
            }

            if (centre.CentreLogo?.Length < 10)
            {
                centre.CentreLogo = null;
            }

            return centre;
        }

        public (IEnumerable<CentreEntity>, int) GetAllCentreSummariesForSuperAdmin(string search, int offset, int rows, int region,
          int centreType,
          int contractType,
          string centreStatus)
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
            }
            string sql = @"SELECT c.CentreID,
                            c.CentreName,
                            c.ContactForename,
                            c.ContactSurname,
                            c.ContactEmail,
                            c.ContactTelephone,
                            c.Active,
                            c.CentreTypeId,
                            ct.CentreType,
                            c.RegionID,
                            r.RegionName
                        FROM Centres AS c
                        INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                        INNER JOIN CentreTypes AS ct ON ct.CentreTypeId = c.CentreTypeId
                        WHERE c.CentreName LIKE N'%' + @search + N'%'
                        AND ((c.RegionID = @region) OR (@region = 0)) AND ((c.CentreTypeId = @centreType) OR (@centreType = 0))
                        AND ((c.ContractTypeID = @contractType) OR (@contractType = 0)) AND ((@centreStatus = 'Any') OR (@centreStatus = 'Active' AND c.Active = 1) OR (@centreStatus = 'Inactive' AND c.Active = 0))
                        ORDER BY LTRIM(c.CentreName)
                            OFFSET @offset ROWS
                            FETCH NEXT @rows ROWS ONLY";
            IEnumerable<CentreEntity> centreEntity = connection.Query<Centre, CentreTypes, Regions, CentreEntity>(
                sql,
                (centre, centreTypes, regions) => new CentreEntity(
                    centre, centreTypes, regions
                ),
                new { search, offset, rows, region, centreType, contractType, centreStatus },
                splitOn: "CentreTypeId,RegionID",
                commandTimeout: 3000
            );
            int resultCount = connection.ExecuteScalar<int>(
                                @$"SELECT  COUNT(*) AS Matches
                                FROM Centres AS c
                                INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                                INNER JOIN CentreTypes AS ct ON ct.CentreTypeId = c.CentreTypeId
                                WHERE c.CentreName LIKE N'%' + @search + N'%' AND ((c.RegionID = @region) OR (@region = 0))  AND ((c.CentreTypeId = @centreType) OR (@centreType = 0)) AND ((c.ContractTypeID = @contractType) OR (@contractType = 0)) AND ((@centreStatus = 'Any') OR (@centreStatus = 'Active' AND c.Active = 1) OR (@centreStatus = 'Inactive' AND c.Active = 0))",
                    new { search, region, centreType, contractType, centreStatus },
                    commandTimeout: 3000
            );
            return (centreEntity, resultCount);
        }

        public IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre()
        {
            return connection.Query<CentreSummaryForFindYourCentre>(
                @"SELECT c.CentreID,
                            c.CentreName,
                            c.RegionID,
                            r.RegionName,
                            c.pwTelephone AS Telephone,
                            c.pwEmail AS Email,
                            c.pwWebURL AS WebUrl,
                            c.pwHours AS Hours,
                            c.pwTrainingLocations AS TrainingLocations,
                            c.pwTrustsCovered AS TrustsCovered,
                            c.pwGeneralInfo AS GeneralInfo,
                            c.kbSelfRegister AS SelfRegister
                        FROM Centres AS c
                        INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                        WHERE c.Active = 1 AND c.ShowOnMap = 1"
            );
        }

        public CentreSummaryForContactDisplay GetCentreSummaryForContactDisplay(int centreId)
        {
            return connection.QueryFirstOrDefault<CentreSummaryForContactDisplay>(
                @"SELECT CentreID,CentreName,pwTelephone AS Telephone,pwEmail AS Email,pwWebURL AS WebUrl,pwHours AS Hours
                         FROM Centres
                         WHERE Active = 1 AND CentreID = @centreId",
                new { centreId }
            );
        }

        public void UpdateCentreManagerDetails(
            int centreId,
            string firstName,
            string lastName,
            string email,
            string? telephone
        )
        {
            connection.Execute(
                @"UPDATE Centres SET
                    ContactForename = @firstName,
                    ContactSurname = @lastName,
                    ContactEmail = @email,
                    ContactTelephone = @telephone
                WHERE CentreId = @centreId",
                new { firstName, lastName, email, telephone, centreId }
            );
        }

        public void UpdateCentreWebsiteDetails(
            int centreId,
            string postcode,
            double latitude,
            double longitude,
            string? telephone = null,
            string? email = null,
            string? openingHours = null,
            string? webAddress = null,
            string? organisationsCovered = null,
            string? trainingVenues = null,
            string? otherInformation = null
        )
        {
            connection.Execute(
                @"UPDATE Centres SET
                    pwTelephone = @telephone,
                    pwEmail = @email,
                    pwPostCode = @postcode,
                    lat = @latitude,
                    long = @longitude,
                    pwHours = @openingHours,
                    pwWebURL = @webAddress,
                    pwTrustsCovered = @organisationsCovered,
                    pwTrainingLocations = @trainingVenues,
                    pwGeneralInfo = @otherInformation
                WHERE CentreId = @centreId",
                new
                {
                    telephone,
                    email,
                    postcode,
                    longitude,
                    latitude,
                    openingHours,
                    webAddress,
                    organisationsCovered,
                    trainingVenues,
                    otherInformation,
                    centreId
                }
            );
        }

        public void UpdateCentreDetails(
            int centreId,
            string? notifyEmail,
            string bannerText,
            byte[]? centreSignature,
            byte[]? centreLogo
        )
        {
            connection.Execute(
                @"UPDATE Centres SET
                    NotifyEmail = @notifyEmail,
                    BannerText = @bannerText,
                    SignatureImage = @centreSignature,
                    CentreLogo = @centreLogo
                WHERE CentreId = @centreId",
                new
                {
                    notifyEmail,
                    bannerText,
                    centreSignature,
                    centreLogo,
                    centreId
                }
            );
        }

        public void UpdateCentreDetailsForSuperAdmin(
            int centreId,
            string centreName,
            int centreTypeId,
            int regionId,
            string? centreEmail,
            string? ipPrefix,
            bool showOnMap
        )
        {
            connection.Execute(
                @"UPDATE Centres SET
                    CentreName = @centreName,
                    CentreTypeId = @centreTypeId,
                    RegionId = @regionId,
                    pwEmail = @centreEmail,
                    IPPrefix = @ipPrefix,
                    ShowOnMap = @showOnMap
                WHERE CentreId = @centreId",
                new
                {
                    centreName,
                    centreTypeId,
                    regionId,
                    centreEmail,
                    ipPrefix,
                    showOnMap,
                    centreId
                }
            );
        }

        public int AddCentreForSuperAdmin(
            string centreName,
            string? contactFirstName,
            string? contactLastName,
            string? contactEmail,
            string? contactPhone,
            int? centreTypeId,
            int? regionId,
            string? registrationEmail,
            string? ipPrefix,
            bool showOnMap,
            bool AddITSPcourses
        )
        {
            int newCentreId;
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();
            {
                newCentreId = connection.QuerySingle<int>(
                @"Insert INTO Centres 
                    (CentreName,
                    ContactForename,
                    ContactSurname,
                    ContactEmail,
                    ContactTelephone,
                    CentreTypeID,
                    RegionID,
                    pwEmail,
                    IPPrefix,
                    ShowOnMap
                    )
                    OUTPUT Inserted.CentreID
                 Values
                    (
                    @centreName,
                    @contactFirstName,
                    @contactLastName,
                    @contactEmail,
                    @contactPhone,
                    @centreTypeId,
                    @regionId,
                    @registrationEmail,
                    @ipPrefix,
                    @showOnMap
                    )",
                new
                {
                    centreName,
                    contactFirstName,
                    contactLastName,
                    contactEmail,
                    contactPhone,
                    centreTypeId,
                    regionId,
                    registrationEmail,
                    ipPrefix,
                    showOnMap
                },
                transaction
            );
                if (AddITSPcourses)
                {
                    connection.Execute(
                        @"INSERT INTO [CentreApplications] ([CentreID], [ApplicationID])
                  SELECT @newCentreId, ApplicationID
                  FROM  Applications
                  WHERE (Debug = 0) AND (ArchivedDate IS NULL) AND (ASPMenu = 1) AND (CoreContent = 1) AND (BrandID = 1) AND (LaunchedAssess = 1)",
                        new { newCentreId },
                        transaction
                    );
                }

                transaction.Commit();
                return newCentreId;
            }

        }

        public (string firstName, string lastName, string email) GetCentreManagerDetails(int centreId)
        {
            var info = connection.QueryFirstOrDefault<(string, string, string)>(
                @"SELECT ContactForename, ContactSurname, ContactEmail
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
            return info;
        }

        public string[] GetCentreIpPrefixes(int centreId)
        {
            var ipPrefixString = connection.QueryFirstOrDefault<string?>(
                @"SELECT IPPrefix
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );

            var ipPrefixes = ipPrefixString?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return ipPrefixes ?? new string[0];
        }

        public (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId)
        {
            return connection.QueryFirstOrDefault<(bool, string?)>(
                @"SELECT AutoRegistered, AutoRegisterManagerEmail
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }

        public IEnumerable<CentreRanking> GetCentreRanks(
            DateTime dateSince,
            int? regionId,
            int resultsCount,
            int centreId
        )
        {
            return connection.Query<CentreRanking>(
                @"WITH SessionsCount AS
                    (
                        SELECT
                            Count(c.CentreID) AS DelegateSessionCount,
                            c.CentreID
                        FROM [Sessions] s
                        INNER JOIN Candidates c ON s.CandidateID = c.CandidateID
                        INNER JOIN Centres ct ON c.CentreID = ct.CentreID
                        WHERE
                            s.LoginTime > @dateSince
                            AND c.CentreID <> 101 AND c.CentreID <> 374
                            AND (ct.RegionID = @RegionID OR @RegionID IS NULL)
                        GROUP BY c.CentreID
                    ),
                    Rankings AS
                    (
                        SELECT
                            RANK() OVER (ORDER BY sc.DelegateSessionCount DESC) AS Ranking,
                            c.CentreID,
                            c.CentreName,
                            sc.DelegateSessionCount
                        FROM SessionsCount sc
                        INNER JOIN Centres c ON sc.CentreID = c.CentreID
                    )
                    SELECT *
                    FROM Rankings
                    WHERE Ranking <= @resultsCount OR CentreID = @centreId
                    ORDER BY Ranking",
                new { dateSince, regionId, resultsCount, centreId }
            );
        }

        public IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap()
        {
            return connection.Query<CentreSummaryForMap>(
                @"SELECT
                        CentreID AS ID,
                        CentreName,
                        Lat AS Latitude,
                        Long AS Longitude,
                        pwTelephone AS Telephone,
                        pwEmail AS Email,
                        pwWebURL AS WebUrl,
                        pwHours AS Hours,
                        pwTrustsCovered AS TrustsCovered,
                        pwTrainingLocations AS TrainingLocations,
                        pwGeneralInfo AS GeneralInfo,
                        kbSelfRegister AS SelfRegister
                    FROM Centres
                    WHERE Active = 1 AND Lat IS NOT NULL AND Long IS NOT NULL AND ShowOnMap = 1"
                );
        }

        public void SetCentreAutoRegistered(int centreId)
        {
            connection.Execute(
                @"UPDATE Centres SET
                    AutoRegistered = 1
                WHERE CentreId = @centreId",
                new { centreId }
            );
        }

        public IEnumerable<(int, string)> GetAllCentres(bool? activeOnly = false)
        {
            var centres = connection.Query<(int, string)>
            (
                @"SELECT CentreID, CentreName
                        FROM Centres
                        WHERE (@activeOnly = 0) OR (Active = 1)
                        ORDER BY CentreName",
                new { activeOnly }
            );
            return centres;
        }

        public IEnumerable<(int, string)> GetCentreTypes()
        {
            return connection.Query<(int, string)>(
                @"SELECT CentreTypeID,CentreType
                   FROM CentreTypes
                   ORDER BY CentreType"
            );
        }

        public Centre? GetCentreManagerDetailsByCentreId(int centreId)
        {
            var centre = connection.QueryFirstOrDefault<Centre>(
                           @"SELECT c.CentreID,
                            c.ContactForename,
                            c.ContactSurname,
                            c.ContactEmail,
                            c.ContactTelephone
                        FROM Centres AS c
                        WHERE c.CentreID = @centreId",
                        new { centreId }
                    );
            return centre;
        }

        public void DeactivateCentre(int centreId)
        {
            connection.Execute(
                @"UPDATE Centres SET
                  Active = 0
                  WHERE CentreId = @centreId",
                new { centreId }
            );
        }

        public void ReactivateCentre(int centreId)
        {
            connection.Execute(
                @"UPDATE Centres SET
                  Active = 1
                  WHERE CentreId = @centreId",
                new { centreId }
            );
        }

        public CentreSummaryForRoleLimits GetRoleLimitsForCentre(int centreId)
        {
            return connection.QueryFirstOrDefault<CentreSummaryForRoleLimits>(
                @"SELECT CentreId,
                        CMSAdministrators AS RoleLimitCMSAdministrators,
                        CMSManagers AS RoleLimitCMSManagers,
                        CCLicences AS RoleLimitCCLicences,
                        CustomCourses AS RoleLimitCustomCourses,
                        Trainers AS RoleLimitTrainers
                        FROM Centres
                        WHERE (CentreId = @centreId) AND (Active = 1)
                        ORDER BY CentreName",
                new { centreId }
            );
        }

        public void UpdateCentreRoleLimits(
            int centreId,
            int? roleLimitCmsAdministrators,
            int? roleLimitCmsManagers,
            int? roleLimitCcLicences,
            int? roleLimitCustomCourses,
            int? roleLimitTrainers
        )
        {
            connection.Execute(
                @"UPDATE Centres SET
                        CMSAdministrators = @roleLimitCMSAdministrators,
                        CMSManagers = @roleLimitCMSManagers,
                        CCLicences = @roleLimitCCLicences,
                        CustomCourses = @roleLimitCustomCourses,
                        Trainers = @roleLimitTrainers
                WHERE CentreId = @centreId",
                new
                {
                    centreId,
                    roleLimitCmsAdministrators,
                    roleLimitCmsManagers,
                    roleLimitCcLicences,
                    roleLimitCustomCourses,
                    roleLimitTrainers,
                }
            );
        }
        public ContractInfo? GetContractInfo(int centreId)
        {
            var centre = connection.QueryFirstOrDefault<ContractInfo>(
                @"SELECT c.CentreID,
                            c.CentreName,
                            ct.ContractTypeID,
                            ct.ContractType,
                            c.ServerSpaceBytes  ServerSpaceBytesInc,
                            c.CandidateByteLimit DelegateUploadSpace,
                             c.ContractReviewDate
					    FROM Centres AS c
                        INNER JOIN ContractTypes AS ct ON ct.ContractTypeID = c.ContractTypeId
                        WHERE CentreID = @centreId",
                new { centreId }
            );
            if (centre == null)
            {
                logger.LogWarning($"No centre found for centre id {centreId}");
                return null;
            }
            return centre;
        }
        public bool UpdateContractTypeandCenter(
           int centreId,
           int contractTypeID,
           long delegateUploadSpace,
          long serverSpaceBytesInc,
          DateTime? contractReviewDate
       )
        {
            var numberOfAffectedRows = connection.Execute(
                @" BEGIN TRY
                    BEGIN TRANSACTION
                        UPDATE Centres SET
                    ServerSpaceBytes = @serverSpaceBytesInc,
                    ContractTypeID = @contractTypeID,
                    ContractReviewDate =@contractReviewDate,
                   CandidateByteLimit=@delegateUploadSpace
                    WHERE CentreID = @centreId

                        COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
                new
                {
                    contractTypeID,
                    delegateUploadSpace,
                    serverSpaceBytesInc,
                    contractReviewDate,
                    centreId
                }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Updating ContraType Information failed. centreId: {centreId} contractTypeID: {contractTypeID} delegateUploadSpace:{delegateUploadSpace}" +
                    $"serverSpaceBytesInc {serverSpaceBytesInc}" +
                    $"contractReviewDate: {contractReviewDate}"
                );
                return false;
            }
            return true;
        }

        public int ResultCount(string search, int region, int centreType, int contractType, string centreStatus)
        {
            int resultCount = connection.ExecuteScalar<int>(
                               @$"SELECT  COUNT(*) AS Matches
                                FROM Centres AS c
                                INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                                INNER JOIN CentreTypes AS ct ON ct.CentreTypeId = c.CentreTypeId
                                WHERE c.CentreName LIKE N'%' + @search + N'%' AND ((c.RegionID = @region) OR (@region = 0))  AND ((c.CentreTypeId = @centreType) OR (@centreType = 0)) AND ((c.ContractTypeID = @contractType) OR (@contractType = 0)) AND ((@centreStatus = 'Any') OR (@centreStatus = 'Active' AND c.Active = 1) OR (@centreStatus = 'Inactive' AND c.Active = 0))",
                   new { search, region, centreType, contractType, centreStatus },
                   commandTimeout: 3000
           );
            return resultCount;
        }

        public IEnumerable<CentresExport> GetAllCentresForSuperAdminExport(string search, int region,
          int centreType, int contractType, string centreStatus, int exportQueryRowLimit, int currentRun)
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
            }
            string sql = @"SELECT
                            CentreID,
                            Active,
                            CentreName,
                            ContactSurname + ', ' + ContactForename AS Contact,          ContactEmail,
                            ContactTelephone,
                            (SELECT RegionName FROM Regions WHERE            (RegionID = c.RegionID)) AS RegionName,
                             (SELECT CentreType FROM CentreTypes
                               WHERE (CentreTypeID = c.CentreTypeID)) AS CentreType,
                            IPPrefix,
                            CentreCreated,
                            (SELECT COUNT(*) AS Expr1 FROM Candidates
                               WHERE (CentreID = c.CentreID)) AS Delegates,
                            (SELECT COUNT(Progress.ProgressID) AS Registrations
                               FROM Progress
                               INNER JOIN                                    Candidates AS Candidates_1 ON Progress.CandidateID = Candidates_1.CandidateID
                               WHERE (Candidates_1.CentreID = c.CentreID)) AS CourseEnrolments,
                             (SELECT COUNT(Progress_1.ProgressID) AS Completions
                               FROM Progress AS Progress_1
                               INNER JOIN
                               Candidates AS Candidates_1 ON Progress_1.CandidateID = Candidates_1.CandidateID
                               WHERE (Progress_1.Completed IS NOT NULL) AND (Candidates_1.CentreID = c.CentreID)) AS CourseCompletions,
                             (SELECT SUM(e.Duration) AS Expr1
                               FROM Sessions AS e INNER JOIN
                               Candidates AS Candidates_2 ON e.CandidateID = Candidates_2.CandidateID
                               WHERE        (Candidates_2.CentreID = c.CentreID)) / 60 AS LearningHours,
                             (SELECT COUNT(*) AS Expr1
                               FROM  AdminUsers
                               WHERE (CentreID = c.CentreID)) AS AdminUsers,
                             (SELECT MAX(ADS.LoginTime) AS Expr1
                               FROM AdminSessions AS ADS INNER JOIN
                               AdminUsers AS ADU ON ADS.AdminID = ADU.AdminID
                               WHERE (ADU.CentreID = c.CentreID)) AS LastAdminLogin,
                             (SELECT MAX(Sessions.LoginTime) AS Expr1
                               FROM Sessions INNER JOIN
                               Customisations ON Sessions.CustomisationID = Customisations.CustomisationID
                               WHERE (Customisations.CentreID = c.CentreID)) AS LastLearnerLogin, 
							   (SELECT ContractType FROM ContractTypes WHERE ContractTypeID = c.ContractTypeID) AS ContractType, 
							   CCLicences, ServerSpaceBytes, 
                         ServerSpaceUsed
                        FROM Centres AS c
                        WHERE c.CentreName LIKE N'%' + @search + N'%'
                        AND ((c.RegionID = @region) OR (@region = 0)) AND ((c.CentreTypeId = @centreType) OR (@centreType = 0))
                        AND ((c.ContractTypeID = @contractType) OR (@contractType = 0)) AND ((@centreStatus = 'Any') OR (@centreStatus = 'Active' AND c.Active = 1) OR (@centreStatus = 'Inactive' AND c.Active = 0))
                        ORDER BY LTRIM(c.CentreName)
                            OFFSET @exportQueryRowLimit * (@currentRun - 1) ROWS
                            FETCH NEXT @exportQueryRowLimit ROWS ONLY";
            IEnumerable<CentresExport> centres = connection.Query<CentresExport>(
                sql,
                new { search, region, centreType, contractType, centreStatus, exportQueryRowLimit, currentRun },
                commandTimeout: 3000
            );

            return centres;
        }
    }
}
