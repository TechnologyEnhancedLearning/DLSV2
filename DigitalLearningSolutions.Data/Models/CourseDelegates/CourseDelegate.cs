namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using System;

    public class CourseDelegate : BaseSearchableItem
    {
        public int DelegateId { get; set; }
        public string CandidateNumber { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? EmailAddress { get; set; }
        public bool Active { get; set; }
        public int ProgressId { get; set; }
        public bool Locked { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public DateTime? Completed { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public int CustomisationId { get; set; }

        public string FullName => (string.IsNullOrEmpty(FirstName) ? "" : $"{FirstName} ") + LastName;

        public string TitleName => FullName + (string.IsNullOrEmpty(EmailAddress) ? "" : $" ({EmailAddress})");

        public bool Removed => RemovedDate.HasValue;

        public double PassRate => AllAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)AllAttempts);

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? FullName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
