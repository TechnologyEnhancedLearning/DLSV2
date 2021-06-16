namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public class CentreRankingViewModel
    {
        public CentreRankingViewModel(IEnumerable<CentreRank> centreRanks, int centreId)
        {
            var centreRanksList = centreRanks.ToList();

            CurrentCentre = centreRanksList.Where(cr => cr.CentreId == centreId)
                .Select(cr => new CentreRankViewModel(cr.Rank, cr.CentreName, cr.Sum))
                .SingleOrDefault();

            TopTenCentres = centreRanksList.OrderBy(cr => cr.Rank).Take(10)
                .Select(cr => new CentreRankViewModel(cr.Rank, cr.CentreName, cr.Sum));
        }

        public IEnumerable<CentreRankViewModel> TopTenCentres { get; set; }

        public CentreRankViewModel? CurrentCentre { get; set; }

        public bool IsCurrentCentreInTopTen => CurrentCentre?.Rank <= 10;
    }
}
