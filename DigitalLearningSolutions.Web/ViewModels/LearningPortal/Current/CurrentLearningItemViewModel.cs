﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
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
            EnrolmentMethodId = course.EnrolmentMethodId;
            ReturnPageQuery = returnPageQuery;
        }

        public DateTime? CompleteByDate { get; }
        public int EnrolmentMethodId { get; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; }
        private readonly IClockUtility clockUtility = new ClockUtility();

        public string DateStyle()
        {
            var utcToday = clockUtility.UtcToday;

            if (CompleteByDate < utcToday)
            {
                return "overdue";
            }

            if (CompleteByDate < utcToday + TimeSpan.FromDays(30))
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
