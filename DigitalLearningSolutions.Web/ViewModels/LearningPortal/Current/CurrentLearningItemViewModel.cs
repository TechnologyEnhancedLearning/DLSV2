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
            Day = course.CompleteByDate?.Day;
            Month = course.CompleteByDate?.Month;
            Year = course.CompleteByDate?.Year;
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateValidator.DateValidationResult? CompleteByValidationResult { get; set; }

        public DateTime? CompleteByDate => Day.HasValue && Month.HasValue && Year.HasValue
            ? new DateTime(Year.Value, Month.Value, Day.Value)
            : (DateTime?)null;

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
                "overdue" => "Course overdue; ",
                "due-soon" => "Course due soon; ",
                _ => "",
            };
        }
    }
}
