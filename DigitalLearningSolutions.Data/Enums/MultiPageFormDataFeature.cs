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
            "EditAssessmentQuestionDataGuid"
        );

        public static readonly MultiPageFormDataFeature EditSignpostingParameter = new MultiPageFormDataFeature(
            7,
            nameof(EditSignpostingParameter),
            "EditSignpostingParameterDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddCompetencyLearningResourceSummary = new MultiPageFormDataFeature(
            8,
            nameof(AddCompetencyLearningResourceSummary),
            "AddCompetencyLearningResourceSummaryDataGuid"
        );

        public static readonly MultiPageFormDataFeature EditCompetencyLearningResources = new MultiPageFormDataFeature(
            9,
            nameof(EditCompetencyLearningResources),
            "EditCompetencyLearningResourcesDataGuid"
        );

        public static readonly MultiPageFormDataFeature SearchInSelfAssessmentOverviewGroups = new MultiPageFormDataFeature(
            10,
            nameof(SearchInSelfAssessmentOverviewGroups),
            "SearchInSelfAssessmentOverviewGroupsDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddSelfAssessmentRequestVerification = new MultiPageFormDataFeature(
            11,
            nameof(AddSelfAssessmentRequestVerification),
            "AddSelfAssessmentRequestVerificationDataGuid"
        );

        public static readonly MultiPageFormDataFeature AddNewSupervisor = new MultiPageFormDataFeature(
            12,
            nameof(AddNewSupervisor),
            "AddNewSupervisorDataGuid"
        );

        public static readonly MultiPageFormDataFeature EnrolDelegateOnProfileAssessment = new MultiPageFormDataFeature(
            13,
            nameof(EnrolDelegateOnProfileAssessment),
            "EnrolDelegateOnProfileAssessmentDataGuid"
        );

        public static readonly MultiPageFormDataFeature EnrolDelegateInActivity = new MultiPageFormDataFeature(
            14,
            nameof(EnrolDelegateInActivity),
            "EnrolDelegateInActivity"
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
