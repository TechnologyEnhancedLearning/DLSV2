namespace DigitalLearningSolutions.Data.Models.DbModels
{
    public class CentreRanking
    {
        public int CentreId { get; set; }

        public int Ranking { get; set; }

        public string CentreName { get; set; } = string.Empty;

        public int DelegateSessionCount { get; set; }
    }
}
