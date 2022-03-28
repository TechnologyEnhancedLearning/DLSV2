namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditDelegateCourseAdminFieldViewModel : EditDelegateCourseAdminFieldFormData
    {
        public EditDelegateCourseAdminFieldViewModel(
            int progressId,
            int promptNumber,
            DelegateCourseDetails details,
            DelegateAccessRoute accessedVia,
            int? returnPage
        ) : base(details, returnPage)
        {
            var courseAdminField = details.CourseAdminFields.Find(c => c.PromptNumber == promptNumber);

            Options = courseAdminField!.Options;
            Answer = GetAnswer(details.DelegateCourseInfo, promptNumber);
            Radios = GetRadioItems(details.DelegateCourseInfo, promptNumber);
            PromptText = courseAdminField.PromptText;
            ProgressId = progressId;
            AccessedVia = accessedVia;
            ReturnPage = returnPage;
        }

        public EditDelegateCourseAdminFieldViewModel(
            EditDelegateCourseAdminFieldFormData formData,
            DelegateCourseDetails details,
            int progressId,
            int promptNumber,
            DelegateAccessRoute accessedVia,
            int? returnPage
        ) : base(formData)
        {
            var courseAdminField = details.CourseAdminFields.Find(c => c.PromptNumber == promptNumber);

            Options = courseAdminField!.Options;
            Radios = GetRadioItems(details.DelegateCourseInfo, promptNumber);
            PromptText = courseAdminField.PromptText;
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public List<string> Options { get; set; }
        public List<RadiosItemViewModel> Radios { get; set; }
        public string PromptText { get; set; }
        public int ProgressId { get; set; }
        public DelegateAccessRoute AccessedVia { get; set; }

        private static string? GetAnswer(DelegateCourseInfo info, int promptNumber)
        {
            return promptNumber switch
            {
                1 => info.Answer1,
                2 => info.Answer2,
                3 => info.Answer3,
                _ => throw new Exception("Invalid prompt number"),
            };
        }

        private List<RadiosItemViewModel> GetRadioItems(DelegateCourseInfo info, int promptNumber)
        {
            var answerOptions = Options.Select(
                option => new RadiosItemViewModel(option, option, option == GetAnswer(info, promptNumber), null)
            ).ToList();
            answerOptions.Add(new RadiosItemViewModel("", "Leave blank", "" == GetAnswer(info, promptNumber), null));
            return answerOptions;
        }
    }
}
