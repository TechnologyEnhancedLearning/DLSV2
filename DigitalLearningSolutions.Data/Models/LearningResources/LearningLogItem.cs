namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using System;

    public class LearningLogItem
    {
        public int LearningLogItemId { get; set; }
        public DateTime LoggedDate { get; set; }
        public int LoggedById { get; set; }
        public string? Activity { get; set; }
        public string? ExternalUri { get; set; }
        public int? LinkedCompetencyLearningResourceId { get; set; }
        public string ActivityType { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int DurationMins { get; set; }
        public string? Outcomes { get; set; }
        public int? LinkedCustomisationId { get; set; }
        public int? VerifiedById { get; set; }
        public string? VerifierComments { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public int? ArchivedById { get; set; }
        public Guid? IcsGuid { get; set; }
        public int? LoggedByAdminId { get; set; }
        public int? SeqInt { get; set; }
        public DateTime? LastAccessedDate { get; set; }
        public int? LearningHubResourceReferenceId { get; set; }
    }
}
