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

        void UpdateCentreManagerDetails(int centreId, string firstName, string lastName,string email, string? telephone );
        string? GetCentreName(int centreId);
        (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId);

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

        public (IEnumerable<CentreEntity>, int) GetAllCentreSummariesForSuperAdmin(string search, int offset, int rows,int region,
          int centreType,
          int contractType,
          string centreStatus)
        {
            return centresDataService.GetAllCentreSummariesForSuperAdmin(search,offset,rows,region,centreType,contractType,centreStatus);
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

        public void UpdateCentreManagerDetails(int centreId,string firstName,string lastName,string email,string? telephone
        )
        {
            centresDataService.UpdateCentreManagerDetails(centreId,firstName,lastName,email,telephone);
        }

        public string? GetCentreName(int centreId)
        {
          return   centresDataService.GetCentreName(centreId);
        }

        public (bool autoRegistered, string? autoRegisterManagerEmail) GetCentreAutoRegisterValues(int centreId)
        {
            return centresDataService.GetCentreAutoRegisterValues(centreId);
        }
    }
}
