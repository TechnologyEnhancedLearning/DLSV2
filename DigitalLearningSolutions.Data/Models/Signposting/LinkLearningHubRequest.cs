namespace DigitalLearningSolutions.Data.Models.Signposting
{
    using System.ComponentModel.DataAnnotations;

    public class LinkLearningHubRequest
    {
        public int UserId { get; set; }

        [Required]
        public string Hash { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        public static string SessionIdentifierKey = "LinkLearningHubRequestIdentifier";
    }
}
