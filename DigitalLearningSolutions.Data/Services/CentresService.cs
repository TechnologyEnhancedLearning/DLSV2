namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public interface ICentresService
    {
        IEnumerable<CentreRank> GetTopCentreRanks(int centreId, int numberOfDays, int regionId);

        int GetCentreRankForCentre(int centreId);
    }

    public class CentresService : ICentresService
    {
        private readonly ICentresDataService centresDataService;

        public CentresService(ICentresDataService centresDataService)
        {
            this.centresDataService = centresDataService;
        }

        public IEnumerable<CentreRank> GetTopCentreRanks(int centreId, int numberOfDays, int regionId)
        {
            var dateSince = DateTime.UtcNow.AddDays(-numberOfDays);

            var centreRanks = centresDataService.GetCentreRanks(dateSince, regionId).OrderBy(cr => cr.Rank).ToList();

            var topTenCentres = centreRanks.Take(10).ToList();

            var currentCentreRank = centreRanks.SingleOrDefault(cr => cr.CentreId == centreId);

            if (currentCentreRank == null)
            {
                return topTenCentres;
            }

            return topTenCentres.Contains(currentCentreRank) ? topTenCentres : topTenCentres.Append(currentCentreRank);
        }

        public int GetCentreRankForCentre(int centreId)
        {
            var centreRanks = centresDataService.GetCentreRanks(DateTime.UtcNow.AddDays(-14), -1);
            var centreRank = centreRanks.SingleOrDefault(cr => cr.CentreId == centreId);
            return centreRank?.Rank ?? -1;
        }
    }
}
