namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;

    public class CurrentLearningItemViewModel : StartedLearningItemViewModel
    {
        public CurrentLearningItemViewModel(
            CurrentLearningItem course,
            ReturnPageQuery returnPageQuery
        ) : base(course)
        {
            CompleteByDate = course.CompleteByDate;
            ReturnPageQuery = returnPageQuery;
        }

        public DateTime? CompleteByDate { get; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; }
        private readonly IClockUtility clockUtility = new ClockUtility();

        public string DateStyle()
        {
            if (CompleteByDate < clockUtility.UtcToday)
            {
                return "overdue";
            }

            if (CompleteByDate < clockUtility.UtcToday + TimeSpan.FromDays(30))
            {
                return "due-soon";
            }

            return "";
        }

        public string DueByDescription()
        {
            return DateStyle() switch
            {
                "overdue" => "Activity overdue; ",
                "due-soon" => "Activity due soon; ",
                _ => "",
            };
        }
    }
}
