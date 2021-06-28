namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking
{
    public class CentreRankViewModel
    {
        public CentreRankViewModel(int rank, string centreName, int sum, bool isHighlighted)
        {
            Rank = rank;
            CentreName = centreName;
            Sum = sum;
            IsHighlighted = isHighlighted;
        }

        public int Rank { get; set; }

        public string CentreName { get; set; }

        public int Sum { get; set; }

        public bool IsHighlighted { get; set; }
    }
}
