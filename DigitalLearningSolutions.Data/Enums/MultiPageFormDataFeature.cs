namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.Linq;

    public class MultiPageFormDataFeature : Enumeration
    {
        public static readonly MultiPageFormDataFeature AddNewCourse = new MultiPageFormDataFeature(
            0,
            nameof(AddNewCourse),
            "AddNewCourseDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddRegistrationPrompt = new MultiPageFormDataFeature(
            1,
            nameof(AddRegistrationPrompt),
            "AddRegistrationPromptDataGuid"
        );

        public static readonly MultiPageFormDataFeature EditRegistrationPrompt = new MultiPageFormDataFeature(
            2,
            nameof(EditRegistrationPrompt),
            "EditRegistrationPromptDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddAdminField = new MultiPageFormDataFeature(
            3,
            nameof(AddAdminField),
            "AddAdminFieldDataGuid"
        );

        public static readonly MultiPageFormDataFeature EditAdminField = new MultiPageFormDataFeature(
            4,
            nameof(EditAdminField),
            "EditAdminFieldDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddNewFramework = new MultiPageFormDataFeature(
            5,
            nameof(AddNewFramework),
            "AddNewFrameworkDataGuid"
        );

        public static readonly MultiPageFormDataFeature EditAssessmentQuestion = new MultiPageFormDataFeature(
            6,
            nameof(EditAssessmentQuestion),
            "EditAssessmentQuestionsDataGuid"
        );

        public readonly string TempDataKey;

        private MultiPageFormDataFeature(int id, string name, string tempDataKey) : base(id, name)
        {
            TempDataKey = tempDataKey;
        }

        public static implicit operator MultiPageFormDataFeature(string value)
        {
            try
            {
                return FromName<MultiPageFormDataFeature>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string?(MultiPageFormDataFeature? applicationType)
        {
            return applicationType?.Name;
        }
    }
}
