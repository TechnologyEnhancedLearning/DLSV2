namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ControllerHelpers;

    public class CurrentLearningItemViewModel : StartedLearningItemViewModel
    {
        public DateTime? CompleteByDate { get; }
        public DateValidator.ValidationResult? CompleteByValidationResult { get; set; }

        public CurrentLearningItemViewModel(
            CurrentLearningItem course
        ) : base(course)
        {
            CompleteByDate = course.CompleteByDate;
        }

        public string DateStyle()
        {
            if (CompleteByDate < DateTime.Today)
            {
                return "overdue";
            }

            if (CompleteByDate < (DateTime.Today + TimeSpan.FromDays(30)))
            {
                return "due-soon";
            }

            return "";
        }

        public string DueByDescription()
        {
            return DateStyle() switch
                {
                "overdue" => "Course overdue; ",
                "due-soon" => "Course due soon; ",
                _ => ""
                };
        }
    }
}
