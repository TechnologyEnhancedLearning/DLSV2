namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt;

    public class MultiPageFormDataFeature : Enumeration
    {
        public static readonly MultiPageFormDataFeature AddNewCourse = new MultiPageFormDataFeature(
            0,
            nameof(AddNewCourse),
            "AddNewCourseDataGuid",
            typeof(AddNewCentreCourseData)
        );

        public static readonly MultiPageFormDataFeature AddRegistrationPrompt = new MultiPageFormDataFeature(
            1,
            nameof(AddRegistrationPrompt),
            "AddRegistrationPromptDataGuid",
            typeof(AddRegistrationPromptData)
        );

        public static readonly MultiPageFormDataFeature EditRegistrationPrompt = new MultiPageFormDataFeature(
            2,
            nameof(EditRegistrationPrompt),
            "EditRegistrationPromptDataGuid",
            typeof(EditRegistrationPromptData)
        );

        public static readonly MultiPageFormDataFeature AddAdminField = new MultiPageFormDataFeature(
            3,
            nameof(AddAdminField),
            "AddAdminFieldDataGuid",
            typeof(AddAdminFieldData)
        );

        public static readonly MultiPageFormDataFeature EditAdminField = new MultiPageFormDataFeature(
            4,
            nameof(EditAdminField),
            "EditAdminFieldDataGuid",
            typeof(EditAdminFieldData)
        );

        public readonly string TempDataKey;

        private readonly Type type;

        private MultiPageFormDataFeature(int id, string name, string tempDataKey, Type type) : base(id, name)
        {
            TempDataKey = tempDataKey;
            this.type = type;
        }

        public static MultiPageFormDataFeature? GetFeatureByType(Type type)
        {
            return GetAll<MultiPageFormDataFeature>().SingleOrDefault(m => m.type == type);
        }
    }
}
