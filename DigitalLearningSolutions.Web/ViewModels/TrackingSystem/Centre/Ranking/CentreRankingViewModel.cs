namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking
{
    using System.Collections.Generic;
    using System.Linq;

    public class CentreRankingViewModel
    {
        public CentreRankingViewModel()
        {
            CurrentCentre = new CentreRankViewModel(23, "Test Centre 13", 123);
            TopTenCentres = new[]
            {
                new CentreRankViewModel(1, "Test Centre 1", 9000),
                new CentreRankViewModel(2, "Test Centre 2", 8000),
                new CentreRankViewModel(3, "Test Centre 3", 7000),
                new CentreRankViewModel(4, "Test Centre 4", 6000),
                new CentreRankViewModel(5, "Test Centre 5", 5000),
                new CentreRankViewModel(6, "Test Centre 6", 4000),
                new CentreRankViewModel(7, "Test Centre 7", 3000),
                new CentreRankViewModel(8, "Test Centre 8", 2000),
                new CentreRankViewModel(9, "Test Centre 9", 1000),
                new CentreRankViewModel(10, "Test Centre 10", 500)
            };
        }

        public IEnumerable<CentreRankViewModel> TopTenCentres { get; set; }

        public CentreRankViewModel CurrentCentre { get; set; }

        public bool IsCurrentCentreInTopTen => TopTenCentres.Contains(CurrentCentre);
    }
}
