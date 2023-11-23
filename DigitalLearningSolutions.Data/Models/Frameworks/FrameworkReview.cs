namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;
    public class FrameworkReview
    {
        public int ID { get; set; }
        public int FrameworkID { get; set; }
        public int FrameworkCollaboratorID { get; set; }
        public string UserEmail { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime ReviewRequested { get; set; }
        public DateTime? ReviewComplete { get; set; }
        public bool SignedOff { get; set; }
        public int? FrameworkCommentID { get; set; }
        public string? Comment { get; set; }
        public bool SignOffRequired { get; set; }
    }
}
