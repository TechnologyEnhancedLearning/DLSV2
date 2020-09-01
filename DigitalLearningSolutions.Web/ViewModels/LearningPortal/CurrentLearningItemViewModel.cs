namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;

    public class CurrentLearningItemViewModel : StartedLearningItemViewModel
    {
        public DateTime? CompleteByDate { get; }

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
