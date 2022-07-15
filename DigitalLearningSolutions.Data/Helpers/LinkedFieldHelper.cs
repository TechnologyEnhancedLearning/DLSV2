﻿namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;

    public static class LinkedFieldHelper
    {
        public static List<LinkedFieldChange> GetLinkedFieldChanges(
            RegistrationFieldAnswers oldAnswers,
            RegistrationFieldAnswers newAnswers,
            IJobGroupsDataService jobGroupsDataService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService
        )
        {
            var changedLinkedFieldsWithAnswers = new List<LinkedFieldChange>();

            if (newAnswers.JobGroupId != oldAnswers.JobGroupId)
            {
                var oldJobGroup = jobGroupsDataService.GetJobGroupName(oldAnswers.JobGroupId);
                var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
                changedLinkedFieldsWithAnswers.Add(new LinkedFieldChange(RegistrationField.JobGroup.LinkedToFieldId, "Job group", oldJobGroup, newJobGroup));
            }

            changedLinkedFieldsWithAnswers = changedLinkedFieldsWithAnswers.Concat(
                AddCustomPromptLinkedFields(oldAnswers, newAnswers, centreRegistrationPromptsService)
            ).ToList();

            return changedLinkedFieldsWithAnswers;
        }

        public static List<LinkedFieldChange> GetNewLinkedFields(
            RegistrationFieldAnswers newAnswers,
            IJobGroupsDataService jobGroupsDataService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService
        )
        {
            var linkedFieldsWithAnswers = new List<LinkedFieldChange>();

            var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
            linkedFieldsWithAnswers.Add(new LinkedFieldChange(RegistrationField.JobGroup.LinkedToFieldId, "Job group", null, newJobGroup));

            var blankAnswers = new RegistrationFieldAnswers(
                newAnswers.CentreId,
                0,
                null,
                null,
                null,
                null,
                null,
                null
            );
            linkedFieldsWithAnswers = linkedFieldsWithAnswers.Concat(
                AddCustomPromptLinkedFields(blankAnswers, newAnswers, centreRegistrationPromptsService)
            ).ToList();

            return linkedFieldsWithAnswers;
        }

        private static IEnumerable<LinkedFieldChange> AddCustomPromptLinkedFields(
            RegistrationFieldAnswers oldAnswers,
            RegistrationFieldAnswers newAnswers,
            ICentreRegistrationPromptsService centreRegistrationPromptsService
        )
        {
            var linkedFieldChanges = new List<LinkedFieldChange>();

            if (newAnswers.Answer1 != oldAnswers.Answer1)
            {
                var prompt1Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField1.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField1.LinkedToFieldId, prompt1Name, oldAnswers.Answer1, newAnswers.Answer1)
                );
            }

            if (newAnswers.Answer2 != oldAnswers.Answer2)
            {
                var prompt2Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField2.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField2.LinkedToFieldId, prompt2Name, oldAnswers.Answer2, newAnswers.Answer2)
                );
            }

            if (newAnswers.Answer3 != oldAnswers.Answer3)
            {
                var prompt3Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField3.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField3.LinkedToFieldId, prompt3Name, oldAnswers.Answer3, newAnswers.Answer3)
                );
            }

            if (newAnswers.Answer4 != oldAnswers.Answer4)
            {
                var prompt4Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField4.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField4.LinkedToFieldId, prompt4Name, oldAnswers.Answer4, newAnswers.Answer4)
                );
            }

            if (newAnswers.Answer5 != oldAnswers.Answer5)
            {
                var prompt5Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField5.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField5.LinkedToFieldId, prompt5Name, oldAnswers.Answer5, newAnswers.Answer5)
                );
            }

            if (newAnswers.Answer6 != oldAnswers.Answer6)
            {
                var prompt6Name =
                    centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(oldAnswers.CentreId, RegistrationField.CentreRegistrationField6.Id);
                linkedFieldChanges.Add(
                    new LinkedFieldChange(RegistrationField.CentreRegistrationField6.LinkedToFieldId, prompt6Name, oldAnswers.Answer6, newAnswers.Answer6)
                );
            }

            return linkedFieldChanges;
        }
    }
}
