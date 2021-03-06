﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using Microsoft.Extensions.Logging;

    public interface ICentresDataService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
        IEnumerable<(int, string)> GetActiveCentresAlphabetical();
        Centre? GetCentreDetailsById(int centreId);

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

        (string firstName, string lastName, string email) GetCentreManagerDetails(int centreId);
        string[] GetCentreIpPrefixes(int centreId);
        (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId);
        void SetCentreAutoRegistered(int centreId);
        IEnumerable<CentreRanking> GetCentreRanks(DateTime dateSince, int? regionId, int resultsCount, int centreId);
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

        public IEnumerable<(int, string)> GetActiveCentresAlphabetical()
        {
            var centres = connection.Query<(int, string)>
            (
                @"SELECT CentreID, CentreName
                        FROM Centres
                        WHERE Active = 1
                        ORDER BY CentreName"
            );
            return centres;
        }

        public Centre? GetCentreDetailsById(int centreId)
        {
            var centre = connection.QueryFirstOrDefault<Centre>(
                @"SELECT c.CentreID,
                            c.CentreName,
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
                            c.ServerSpaceBytes
                        FROM Centres AS c
                        INNER JOIN Regions AS r ON r.RegionID = c.RegionID
                        INNER JOIN ContractTypes AS ct ON ct.ContractTypeID = c.ContractTypeId
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

        public void SetCentreAutoRegistered(int centreId)
        {
            connection.Execute(
                @"UPDATE Centres SET
                    AutoRegistered = 1
                WHERE CentreId = @centreId",
                new { centreId }
            );
        }
    }
}
