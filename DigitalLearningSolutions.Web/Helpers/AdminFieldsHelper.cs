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
        public static List<DelegateCourseAdminField> GetCourseAdminFieldViewModels(
            CourseDelegate courseDelegate,
            IEnumerable<CourseAdminField> courseAdminFields
        )
        {
            var answers = new List<string?>
            {
                courseDelegate.Answer1,
                courseDelegate.Answer2,
                courseDelegate.Answer3,
            };

            return courseAdminFields.Select(
                c => new DelegateCourseAdminField(
                    c.PromptNumber,
                    c.PromptText,
                    answers[c.PromptNumber - 1]
                )
            ).ToList();
        }
    }
}
