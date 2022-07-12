namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Services;

    public static class LinkedFieldHelper
    {
        public static List<LinkedFieldChange> GetLinkedFieldChanges(
            CentreAnswersData oldAnswers,
            CentreAnswersData newAnswers,
            IJobGroupsDataService jobGroupsDataService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService
        )
        {
            var changedLinkedFieldsWithAnswers = new List<LinkedFieldChange>();

            if (newAnswers.Answer1 != oldAnswers.Answer1)
            {
                var prompt1Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField1.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField1.LinkedToFieldId, prompt1Name, oldAnswers.Answer1, newAnswers.Answer1)
                );
            }

            if (newAnswers.Answer2 != oldAnswers.Answer2)
            {
                var prompt2Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField2.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField2.LinkedToFieldId, prompt2Name, oldAnswers.Answer2, newAnswers.Answer2)
                );
            }

            if (newAnswers.Answer3 != oldAnswers.Answer3)
            {
                var prompt3Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField3.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField3.LinkedToFieldId, prompt3Name, oldAnswers.Answer3, newAnswers.Answer3)
                );
            }

            if (newAnswers.JobGroupId != oldAnswers.JobGroupId)
            {
                var oldJobGroup = jobGroupsDataService.GetJobGroupName(oldAnswers.JobGroupId);
                var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
                changedLinkedFieldsWithAnswers.Add(new LinkedFieldChange(RegistrationField.JobGroup.LinkedToFieldId, "Job group", oldJobGroup, newJobGroup));
            }

            if (newAnswers.Answer4 != oldAnswers.Answer4)
            {
                var prompt4Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField4.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField4.LinkedToFieldId, prompt4Name, oldAnswers.Answer4, newAnswers.Answer4)
                );
            }

            if (newAnswers.Answer5 != oldAnswers.Answer5)
            {
                var prompt5Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField5.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField5.LinkedToFieldId, prompt5Name, oldAnswers.Answer5, newAnswers.Answer5)
                );
            }

            if (newAnswers.Answer6 != oldAnswers.Answer6)
            {
                var prompt6Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField6.Id);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField6.LinkedToFieldId, prompt6Name, oldAnswers.Answer6, newAnswers.Answer6)
                );
            }

            return changedLinkedFieldsWithAnswers;
        }
    }
}
