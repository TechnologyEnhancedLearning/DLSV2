namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LinkedFieldHelper
    {
        public static List<LinkedFieldChange> GetLinkedFieldChanges(
            CentreAnswersData oldAnswers,
            CentreAnswersData newAnswers,
            IJobGroupsDataService jobGroupsDataService
        )
        {
            var changedLinkedFieldsWithAnswers = new List<LinkedFieldChange>();

            if (newAnswers.Answer1 != oldAnswers.Answer1)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(1, oldAnswers.Answer1, newAnswers.Answer1)
                );
            }

            if (newAnswers.Answer2 != oldAnswers.Answer2)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(2, oldAnswers.Answer2, newAnswers.Answer2)
                );
            }

            if (newAnswers.Answer3 != oldAnswers.Answer3)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(3, oldAnswers.Answer3, newAnswers.Answer3)
                );
            }

            if (newAnswers.JobGroupId != oldAnswers.JobGroupId)
            {
                var oldJobGroup = jobGroupsDataService.GetJobGroupName(oldAnswers.JobGroupId);
                var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
                changedLinkedFieldsWithAnswers.Add(new LinkedFieldChange(4, oldJobGroup, newJobGroup));
            }

            if (newAnswers.Answer4 != oldAnswers.Answer4)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(5, oldAnswers.Answer4, newAnswers.Answer4)
                );
            }

            if (newAnswers.Answer5 != oldAnswers.Answer5)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(6, oldAnswers.Answer5, newAnswers.Answer5)
                );
            }

            if (newAnswers.Answer6 != oldAnswers.Answer6)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldChange(7, oldAnswers.Answer6, newAnswers.Answer6)
                );
            }

            return changedLinkedFieldsWithAnswers;
        }
    }
}
