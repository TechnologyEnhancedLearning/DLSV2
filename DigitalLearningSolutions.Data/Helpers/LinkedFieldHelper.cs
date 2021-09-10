namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LinkedFieldHelper
    {
        public static List<LinkedFieldNumberWithValues> GetLinkedFieldNumbersWithValuesFromChangedAnswers(
            DelegateUser oldDelegateDetails,
            CentreAnswersData newAnswers,
            IJobGroupsDataService jobGroupsDataService
        )
        {
            var changedLinkedFieldsWithAnswers = new List<LinkedFieldNumberWithValues>();

            if (newAnswers.Answer1 != oldDelegateDetails.Answer1)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(1, oldDelegateDetails.Answer1, newAnswers.Answer1)
                );
            }

            if (newAnswers.Answer2 != oldDelegateDetails.Answer2)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(2, oldDelegateDetails.Answer2, newAnswers.Answer2)
                );
            }

            if (newAnswers.Answer3 != oldDelegateDetails.Answer3)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(3, oldDelegateDetails.Answer3, newAnswers.Answer3)
                );
            }

            if (newAnswers.JobGroupId != oldDelegateDetails.JobGroupId)
            {
                var oldJobGroup = jobGroupsDataService.GetJobGroupName(oldDelegateDetails.JobGroupId);
                var newJobGroup = jobGroupsDataService.GetJobGroupName(newAnswers.JobGroupId);
                changedLinkedFieldsWithAnswers.Add(new LinkedFieldNumberWithValues(4, oldJobGroup, newJobGroup));
            }

            if (newAnswers.Answer4 != oldDelegateDetails.Answer4)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(5, oldDelegateDetails.Answer4, newAnswers.Answer4)
                );
            }

            if (newAnswers.Answer5 != oldDelegateDetails.Answer5)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(6, oldDelegateDetails.Answer5, newAnswers.Answer5)
                );
            }

            if (newAnswers.Answer6 != oldDelegateDetails.Answer6)
            {
                changedLinkedFieldsWithAnswers.Add(
                    new LinkedFieldNumberWithValues(7, oldDelegateDetails.Answer6, newAnswers.Answer6)
                );
            }

            return changedLinkedFieldsWithAnswers;
        }
    }
}
