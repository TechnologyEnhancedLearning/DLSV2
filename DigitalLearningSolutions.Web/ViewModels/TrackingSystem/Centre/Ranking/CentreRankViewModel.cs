namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking
{
    public class CentreRankViewModel
    {
        public CentreRankViewModel(int rank, string centreName, int sum)
        {
            Rank = rank;
            CentreName = centreName;
            Sum = sum;
        }

        public int Rank { get; set; }

        public string CentreName { get; set; }

        public int Sum { get; set; }
    }
}
