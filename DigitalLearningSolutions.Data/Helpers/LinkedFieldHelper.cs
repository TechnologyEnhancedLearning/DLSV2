namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;

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
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 1);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(1, prompt1Name, oldAnswers.Answer1, newAnswers.Answer1)
                );
            }

            if (newAnswers.Answer2 != oldAnswers.Answer2)
            {
                var prompt2Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 2);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(2, prompt2Name, oldAnswers.Answer2, newAnswers.Answer2)
                );
            }

            if (newAnswers.Answer3 != oldAnswers.Answer3)
            {
                var prompt3Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 3);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(3, prompt3Name, oldAnswers.Answer3, newAnswers.Answer3)
                );
            }

            if (newAnswers.JobGroupId != oldAnswers.JobGroupId)
            {
                var oldJobGroup = jobGroupsDataService.GetJobGroupName(oldAnswers.JobGroupId);
                var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
                changedLinkedFieldsWithAnswers.Add(new LinkedFieldChange(4, "Job Group", oldJobGroup, newJobGroup));
            }

            if (newAnswers.Answer4 != oldAnswers.Answer4)
            {
                var prompt4Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 4);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(5, prompt4Name, oldAnswers.Answer4, newAnswers.Answer4)
                );
            }

            if (newAnswers.Answer5 != oldAnswers.Answer5)
            {
                var prompt5Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 5);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(6, prompt5Name, oldAnswers.Answer5, newAnswers.Answer5)
                );
            }

            if (newAnswers.Answer6 != oldAnswers.Answer6)
            {
                var prompt6Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, 6);
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(7, prompt6Name, oldAnswers.Answer6, newAnswers.Answer6)
                );
            }

            return changedLinkedFieldsWithAnswers;
        }
    }
}
