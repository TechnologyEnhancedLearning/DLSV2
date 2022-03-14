namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class LearningLogViewModel : BaseSearchablePageViewModel<LearningLogEntry>
    {
        public LearningLogViewModel(
            DelegateProgressAccessRoute accessedVia,
            LearningLog learningLog,
            SearchSortFilterPaginationResult<LearningLogEntry> result
        ) : base(result, false)
        {
            AccessedVia = accessedVia;
            ProgressId = learningLog.DelegateCourseInfo.ProgressId;
            CustomisationId = learningLog.DelegateCourseInfo.CustomisationId;
            CourseName = learningLog.DelegateCourseInfo.CourseName;
            DelegateId = learningLog.DelegateCourseInfo.DelegateId;
            DelegateFirstName = learningLog.DelegateCourseInfo.DelegateFirstName;
            DelegateLastName = learningLog.DelegateCourseInfo.DelegateLastName;
            DelegateEmail = learningLog.DelegateCourseInfo.DelegateEmail;

            Entries = result.ItemsToDisplay.Select(entry => new LearningLogEntryViewModel(entry));
        }

        public DelegateProgressAccessRoute AccessedVia { get; set; }
        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public IEnumerable<LearningLogEntryViewModel> Entries { get; set; }

        public string DelegateFullName =>
            DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(DelegateFirstName, DelegateLastName);

        public string DelegateNameAndEmail =>
            string.IsNullOrWhiteSpace(DelegateEmail) ? DelegateFullName : $"{DelegateFullName} ({DelegateEmail})";

        public override IEnumerable<(string, string)> SortOptions => new[]
        {
            LearningLogSortByOptions.When,
            LearningLogSortByOptions.LearningTime,
            LearningLogSortByOptions.AssessmentScore,
        };

        public override bool NoDataFound => !Entries.Any();
    }
}
