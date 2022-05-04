namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

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
        public DateTime Registered { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public DateTime? Completed { get; set; }
        public bool HasCompleted => Completed.HasValue;
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public int CustomisationId { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public bool? SupervisorAdminActive { get; set; }
        public bool IsAssessed { get; set; }
        public DateTime? Evaluated { get; set; }
        public int EnrolmentMethodId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public bool? EnrolledByAdminActive { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }

        public string FullNameForSearchingSorting => NameQueryHelper.GetSortableFullName(FirstName, LastName);

        public bool Removed => RemovedDate.HasValue;

        public double PassRate => AllAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)AllAttempts);

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? FullNameForSearchingSorting;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public override string?[] SearchableContent => new[] { SearchableName, EmailAddress, CandidateNumber };

        public List<CourseAdminFieldWithAnswer> CourseAdminFields { get; set; } =
            new List<CourseAdminFieldWithAnswer>();

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
