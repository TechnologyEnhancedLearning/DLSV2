namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;

    public class CourseDelegate : BaseSearchableItem
    {
        public int DelegateId { get; set; }
        public string CandidateNumber { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? EmailAddress { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool Active { get; set; }
        public int ProgressId { get; set; }
        public bool Locked { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public DateTime? Completed { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public int CustomisationId { get; set; }

        public string FullNameForSearchingSorting => NameQueryHelper.GetSortableFullName(FirstName, LastName);

        public bool Removed => RemovedDate.HasValue;

        public double PassRate => AllAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)AllAttempts);

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? FullNameForSearchingSorting;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public static string GetPropertyNameForAdminFieldAnswer(int coursePromptNumber)
        {
            return coursePromptNumber switch
            {
                1 => nameof(Answer1),
                2 => nameof(Answer2),
                3 => nameof(Answer3),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
