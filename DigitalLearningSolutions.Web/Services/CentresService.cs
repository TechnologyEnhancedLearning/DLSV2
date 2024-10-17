namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Utilities;

    public interface ICentresService
    {
        IEnumerable<CentreRanking> GetCentresForCentreRankingPage(int centreId, int numberOfDays, int? regionId);

        int? GetCentreRankForCentre(int centreId);

        (IEnumerable<CentreEntity>, int) GetAllCentreSummariesForSuperAdmin(string search, int offset, int rows, int region,
          int centreType,
          int contractType,
          string centreStatus);

        IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre();

        CentreSummaryForContactDisplay GetCentreSummaryForContactDisplay(int centreId);

        IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap();

        bool IsAnEmailValidForCentreManager(string? primaryEmail, string? centreSpecificEmail, int centreId);

        void DeactivateCentre(int centreId);

        void ReactivateCentre(int centreId);

        Centre? GetCentreManagerDetailsByCentreId(int centreId);
        void UpdateCentreManagerDetails(
            int centreId,
            string firstName,
            string lastName,
            string email,
            string? telephone
        );
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
        IEnumerable<(int, string)> GetCentresForDelegateSelfRegistrationAlphabetical();
        (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId);
        Centre? GetCentreDetailsById(int centreId);
        IEnumerable<(int, string)> GetCentreTypes();
        Centre? GetFullCentreDetailsById(int centreId);
        IEnumerable<(int, string)> GetAllCentres(bool? activeOnly = false);
        public void UpdateCentreDetailsForSuperAdmin(
            int centreId,
            string centreName,
            int centreTypeId,
            int regionId,
            string? centreEmail,
            string? ipPrefix,
            bool showOnMap
        );
        CentreSummaryForRoleLimits GetRoleLimitsForCentre(int centreId);
        void UpdateCentreRoleLimits(
            int centreId,
            int? roleLimitCmsAdministrators,
            int? roleLimitCmsManagers,
            int? roleLimitCcLicences,
            int? roleLimitCustomCourses,
            int? roleLimitTrainers
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
        ContractInfo? GetContractInfo(int centreId);
        bool UpdateContractTypeandCenter(int centreId, int contractTypeID, long delegateUploadSpace, long serverSpaceBytesInc, DateTime? contractReviewDate);
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
        CentreEntity UpdateCentreWithCounts(CentreEntity item);
    }

    public class CentresService : ICentresService
    {
        private const int NumberOfCentresToDisplay = 10;
        private const int DefaultNumberOfDaysFilter = 14;
        private readonly ICentresDataService centresDataService;
        private readonly IClockUtility clockUtility;

        public CentresService(ICentresDataService centresDataService, IClockUtility clockUtility)
        {
            this.centresDataService = centresDataService;
            this.clockUtility = clockUtility;
        }

        public IEnumerable<CentreRanking> GetCentresForCentreRankingPage(int centreId, int numberOfDays, int? regionId)
        {
            var dateSince = clockUtility.UtcNow.AddDays(-numberOfDays);

            return centresDataService.GetCentreRanks(dateSince, regionId, NumberOfCentresToDisplay, centreId).ToList();
        }

        public int? GetCentreRankForCentre(int centreId)
        {
            var dateSince = clockUtility.UtcNow.AddDays(-DefaultNumberOfDaysFilter);
            var centreRankings = centresDataService.GetCentreRanks(dateSince, null, NumberOfCentresToDisplay, centreId);
            var centreRanking = centreRankings.SingleOrDefault(cr => cr.CentreId == centreId);
            return centreRanking?.Ranking;
        }

        public (IEnumerable<CentreEntity>, int) GetAllCentreSummariesForSuperAdmin(string search, int offset, int rows, int region,
          int centreType,
          int contractType,
          string centreStatus)
        {
            return centresDataService.GetAllCentreSummariesForSuperAdmin(search, offset, rows, region, centreType, contractType, centreStatus);
        }

        public IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre()
        {
            return centresDataService.GetAllCentreSummariesForFindCentre();
        }

        public CentreSummaryForContactDisplay GetCentreSummaryForContactDisplay(int centreId)
        {
            return centresDataService.GetCentreSummaryForContactDisplay(centreId);
        }

        public IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap()
        {
            return centresDataService.GetAllCentreSummariesForMap();
        }

        public bool IsAnEmailValidForCentreManager(string? primaryEmail, string? centreSpecificEmail, int centreId)
        {
            var autoRegisterManagerEmail =
                centresDataService.GetCentreAutoRegisterValues(centreId).autoRegisterManagerEmail;

            return new List<string?> { primaryEmail, centreSpecificEmail }.Any(
                email => email != null && string.Equals(
                    email,
                    autoRegisterManagerEmail,
                    StringComparison.CurrentCultureIgnoreCase
                )
            );
        }

        public void DeactivateCentre(int centreId)
        {
            centresDataService.DeactivateCentre(centreId);
        }

        public void ReactivateCentre(int centreId)
        {
            centresDataService.ReactivateCentre(centreId);
        }

        public Centre? GetCentreManagerDetailsByCentreId(int centreId)
        {
            return centresDataService.GetCentreManagerDetailsByCentreId(centreId);
        }

        public void UpdateCentreManagerDetails(int centreId, string firstName, string lastName, string email, string? telephone
        )
        {
            centresDataService.UpdateCentreManagerDetails(centreId, firstName, lastName, email, telephone);
        }

        public string? GetBannerText(int centreId)
        {
            return centresDataService.GetBannerText(centreId);
        }

        public string? GetCentreName(int centreId)
        {
            return centresDataService.GetCentreName(centreId);
        }
        public IEnumerable<(int, string)> GetCentresForDelegateSelfRegistrationAlphabetical()
        {
            return centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical();
        }
        public (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId)
        {
            return centresDataService.GetCentreAutoRegisterValues(centreId);
        }

        public Centre? GetCentreDetailsById(int centreId)
        {
            return centresDataService.GetCentreDetailsById(centreId);
        }

        public IEnumerable<(int, string)> GetCentreTypes()
        {
            return centresDataService.GetCentreTypes();
        }

        public Centre? GetFullCentreDetailsById(int centreId)
        {
            return centresDataService.GetFullCentreDetailsById(centreId);
        }

        public IEnumerable<(int, string)> GetAllCentres(bool? activeOnly = false)
        {
            return centresDataService.GetAllCentres(activeOnly);
        }

        public void UpdateCentreDetailsForSuperAdmin(int centreId, string centreName, int centreTypeId, int regionId, string? centreEmail, string? ipPrefix, bool showOnMap)
        {
            centresDataService.UpdateCentreDetailsForSuperAdmin(centreId, centreName, centreTypeId, regionId, centreEmail, ipPrefix, showOnMap);
        }

        public CentreSummaryForRoleLimits GetRoleLimitsForCentre(int centreId)
        {
            return centresDataService.GetRoleLimitsForCentre(centreId);
        }

        public void UpdateCentreRoleLimits(int centreId, int? roleLimitCmsAdministrators, int? roleLimitCmsManagers, int? roleLimitCcLicences, int? roleLimitCustomCourses, int? roleLimitTrainers)
        {
            centresDataService.UpdateCentreRoleLimits(centreId, roleLimitCmsAdministrators, roleLimitCmsManagers, roleLimitCcLicences, roleLimitCustomCourses, roleLimitTrainers);
        }

        public int AddCentreForSuperAdmin(string centreName, string? contactFirstName, string? contactLastName, string? contactEmail, string? contactPhone, int? centreTypeId, int? regionId, string? registrationEmail, string? ipPrefix, bool showOnMap, bool AddITSPcourses)
        {
            return centresDataService.AddCentreForSuperAdmin(centreName, contactFirstName, contactLastName, contactEmail, contactPhone, centreTypeId, regionId, registrationEmail, ipPrefix, showOnMap, AddITSPcourses);
        }

        public ContractInfo? GetContractInfo(int centreId)
        {
            return centresDataService.GetContractInfo(centreId);
        }

        public bool UpdateContractTypeandCenter(int centreId, int contractTypeID, long delegateUploadSpace, long serverSpaceBytesInc, DateTime? contractReviewDate)
        {
            return centresDataService.UpdateContractTypeandCenter(centreId, contractTypeID, delegateUploadSpace, serverSpaceBytesInc, contractReviewDate);
        }
        public void UpdateCentreWebsiteDetails(int centreId, string postcode, double latitude, double longitude, string? telephone, string email, string? openingHours, string? webAddress, string? organisationsCovered, string? trainingVenues, string? otherInformation)
        {
            centresDataService.UpdateCentreWebsiteDetails(centreId, postcode, latitude, longitude, telephone, email, openingHours, webAddress, organisationsCovered, trainingVenues, otherInformation);
        }

        public void UpdateCentreDetails(int centreId, string? notifyEmail, string bannerText, byte[]? centreSignature, byte[]? centreLogo)
        {
            centresDataService.UpdateCentreDetails(centreId, notifyEmail, bannerText, centreSignature, centreLogo);
        }
        public  CentreEntity UpdateCentreWithCounts(CentreEntity item)
        {
            var count = this.centresDataService.CountRegisterUserByCentreId(item.Centre.CentreId);
            item.Centre.RegisterUser = count.RegisterUser;
            item.Centre.AutoRegisterManagerEmail = count.AutoRegisterManagerEmail;
            return item;
        }
    }
}
