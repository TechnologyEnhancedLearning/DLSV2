namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class AdminFieldsHelper
    {
        public static List<CustomFieldViewModel> GetCourseAdminFieldViewModels(
            CourseDelegate courseDelegate,
            IEnumerable<CustomPrompt> customPrompts
        )
        {
            var answers = new List<string?>
            {
                courseDelegate.Answer1,
                courseDelegate.Answer2,
                courseDelegate.Answer3,
            };

            return customPrompts.Select(
                cp => new CustomFieldViewModel(
                    cp.CustomPromptNumber,
                    cp.CustomPromptText,
                    cp.Mandatory,
                    answers[cp.CustomPromptNumber - 1]
                )
            ).ToList();
        }

        public static string GetAdminFieldAnswerName(int customPromptNumber)
        {
            return customPromptNumber switch
            {
                1 => nameof(CourseDelegate.Answer1),
                2 => nameof(CourseDelegate.Answer2),
                3 => nameof(CourseDelegate.Answer3),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
