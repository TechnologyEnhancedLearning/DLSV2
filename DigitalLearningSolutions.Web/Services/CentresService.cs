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

        IEnumerable<CentreSummaryForSuperAdmin> GetAllCentreSummariesForSuperAdmin();

        IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre();

        IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap();

        bool DoEmailsMatchCentre(string primaryEmail, string? centreSpecificEmail, int centreId);
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

        public IEnumerable<CentreSummaryForSuperAdmin> GetAllCentreSummariesForSuperAdmin()
        {
            return centresDataService.GetAllCentreSummariesForSuperAdmin();
        }

        public IEnumerable<CentreSummaryForFindYourCentre> GetAllCentreSummariesForFindCentre()
        {
            return centresDataService.GetAllCentreSummariesForFindCentre();
        }

        public IEnumerable<CentreSummaryForMap> GetAllCentreSummariesForMap()
        {
            return centresDataService.GetAllCentreSummariesForMap();
        }

        public bool DoEmailsMatchCentre(string primaryEmail, string? centreSpecificEmail, int centreId)
        {
            var autoRegisterManagerEmail =
                centresDataService.GetCentreAutoRegisterValues(centreId).autoRegisterManagerEmail;

            return new List<string?> { primaryEmail, centreSpecificEmail }.Where(email => email != null).Any(
                email => string.Equals(email, autoRegisterManagerEmail, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
