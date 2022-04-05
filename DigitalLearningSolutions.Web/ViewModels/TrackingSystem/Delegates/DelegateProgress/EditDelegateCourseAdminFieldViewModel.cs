namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditDelegateCourseAdminFieldViewModel : EditDelegateCourseAdminFieldFormData
    {
        public EditDelegateCourseAdminFieldViewModel(
            int progressId,
            int promptNumber,
            DelegateCourseDetails details,
            DelegateAccessRoute accessedVia
        ) : base(details)
        {
            var courseAdminField = details.CourseAdminFields.Single(c => c.PromptNumber == promptNumber);

            Options = courseAdminField.Options;
            Answer = details.DelegateCourseInfo.GetAnswer(promptNumber);
            Radios = GetRadioItems(details.DelegateCourseInfo, promptNumber);
            PromptText = courseAdminField.PromptText;
            CourseName = details.DelegateCourseInfo.CourseName;
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public EditDelegateCourseAdminFieldViewModel(
            EditDelegateCourseAdminFieldFormData formData,
            DelegateCourseDetails details,
            int progressId,
            int promptNumber,
            DelegateAccessRoute accessedVia
        ) : base(formData)
        {
            var courseAdminField = details.CourseAdminFields.Single(c => c.PromptNumber == promptNumber);

            Options = courseAdminField!.Options;
            Radios = GetRadioItems(details.DelegateCourseInfo, promptNumber);
            PromptText = courseAdminField.PromptText;
            CourseName = details.DelegateCourseInfo.CourseName;
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public List<string> Options { get; set; }
        public List<RadiosItemViewModel> Radios { get; set; }
        public string PromptText { get; set; }
        public string? CourseName { get; set; }

        public string DelegateName { get; set; }
        public int ProgressId { get; set; }
        public DelegateAccessRoute AccessedVia { get; set; }

        private List<RadiosItemViewModel> GetRadioItems(DelegateCourseInfo info, int promptNumber)
        {
            var answerOptions = Options.Select(
                option => new RadiosItemViewModel(option, option, option == info.GetAnswer(promptNumber), null)
            ).ToList();
            var noOptionIsSelected = answerOptions.All(ao => !ao.Selected);
            answerOptions.Add(new RadiosItemViewModel(string.Empty, "Leave blank", noOptionIsSelected, null));
            return answerOptions;
        }
    }
}
