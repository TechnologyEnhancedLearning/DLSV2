﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public interface ICentresService
    {
        IEnumerable<CentreRanking> GetCentresForCentreRankingPage(int centreId, int numberOfDays, int? regionId);

        int? GetCentreRankForCentre(int centreId);

        IEnumerable<Centre> GetAllCentreSummaries();
    }

    public class CentresService : ICentresService
    {
        private const int NumberOfCentresToDisplay = 10;
        private const int DefaultNumberOfDaysFilter = 14;
        private readonly ICentresDataService centresDataService;
        private readonly IClockService clockService;

        public CentresService(ICentresDataService centresDataService, IClockService clockService)
        {
            this.centresDataService = centresDataService;
            this.clockService = clockService;
        }

        public IEnumerable<CentreRanking> GetCentresForCentreRankingPage(int centreId, int numberOfDays, int? regionId)
        {
            var dateSince = clockService.UtcNow.AddDays(-numberOfDays);

            return centresDataService.GetCentreRanks(dateSince, regionId, NumberOfCentresToDisplay, centreId).ToList();
        }

        public int? GetCentreRankForCentre(int centreId)
        {
            var dateSince = clockService.UtcNow.AddDays(-DefaultNumberOfDaysFilter);
            var centreRankings = centresDataService.GetCentreRanks(dateSince, null, NumberOfCentresToDisplay, centreId);
            var centreRanking = centreRankings.SingleOrDefault(cr => cr.CentreId == centreId);
            return centreRanking?.Ranking;
        }

        public IEnumerable<Centre> GetAllCentreSummaries()
        {
            return centresDataService.GetAllCentreSummaries();
        }
    }
}
