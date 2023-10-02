namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    public class Catalogue
    {
        public Catalogue()
        {

        }
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsRestricted { get; set; }
    }
}
