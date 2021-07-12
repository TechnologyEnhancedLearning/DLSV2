namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public class CentreRankingViewModel
    {
        public CentreRankingViewModel(IEnumerable<CentreRanking> centreRanks, int centreId, int? regionId, int? period)
        {
            var centreRanksList = centreRanks.ToList();

            Centres = centreRanksList.OrderBy(cr => cr.Ranking)
                .Select(
                    cr => new CentreRankViewModel(
                        cr.Ranking,
                        cr.CentreName,
                        cr.DelegateSessionCount,
                        cr.CentreId == centreId
                    )
                );

            CentreHasNoActivity = centreRanksList.All(cr => cr.CentreId != centreId);
        }

        public IEnumerable<CentreRankViewModel> Centres { get; set; }

        public bool CentreHasNoActivity { get; set; }

        public int? RegionId { get; set; }

        public int? Period { get; set; }
    }
}
