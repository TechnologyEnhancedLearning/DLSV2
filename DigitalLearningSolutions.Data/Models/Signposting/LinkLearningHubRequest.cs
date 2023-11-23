namespace DigitalLearningSolutions.Data.Models.Signposting
{
    using System.ComponentModel.DataAnnotations;

    public class LinkLearningHubRequest
    {
        public int UserId { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        public string State { get; set; }

        public static string SessionIdentifierKey = "LinkLearningHubRequestIdentifier";
    }
}
