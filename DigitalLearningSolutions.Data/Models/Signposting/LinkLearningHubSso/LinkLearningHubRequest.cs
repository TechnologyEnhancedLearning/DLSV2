namespace DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LinkLearningHubRequest
    {
        public int UserId { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        public string State { get; set; }

        public Guid? SessionIdentifier { get; set; }

        public static string SessionIdentifierKey = "LinkLearningHubRequestIdentifier";
    }
}
