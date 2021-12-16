namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class CurrentLearningItemViewModel : StartedLearningItemViewModel
    {
        public CurrentLearningItemViewModel(
            CurrentLearningItem course
        ) : base(course)
        {
            CompleteByDate = course.CompleteByDate;
        }

        public DateTime? CompleteByDate { get; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }

        public string DateStyle()
        {
            if (CompleteByDate < DateTime.Today)
            {
                return "overdue";
            }

            if (CompleteByDate < DateTime.Today + TimeSpan.FromDays(30))
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
