namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public static class GroupTestHelper
    {
        public static GroupDelegate GetDefaultGroupDelegate(
            int groupDelegateId = 62,
            int groupId = 5,
            int delegateId = 245969,
            string? firstName = "xxxxx",
            string lastName = "xxxx",
            string? emailAddress = "gslectik.m@vao",
            string candidateNumber = "KT553"
        )
        {
            return new GroupDelegate
            {
                GroupDelegateId = groupDelegateId,
                GroupId = groupId,
                DelegateId = delegateId,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                CandidateNumber = candidateNumber
            };
        }

        public static GroupCourse GetDefaultGroupCourse(
            int groupCustomisationId = 1,
            int groupId = 8,
            int customisationId = 25918,
            string applicationName = "Practice Nurse Clinical Supervision",
            string? customisationName = "Demo",
            bool isMandatory = false,
            bool isAssessed = false,
            DateTime? addedToGroup = null,
            string? supervisorFirstName = null,
            string? supervisorLastName = null,
            int completeWithinMonths = 12,
            int validityMonths = 0
        )
        {
            return new GroupCourse
            {
                GroupCustomisationId = groupCustomisationId,
                GroupId = groupId,
                CustomisationId = customisationId,
                ApplicationName = applicationName,
                CustomisationName = customisationName,
                IsMandatory = isMandatory,
                IsAssessed = isAssessed,
                AddedToGroup = addedToGroup ?? DateTime.Now,
                SupervisorFirstName = supervisorFirstName,
                SupervisorLastName = supervisorLastName,
                CompleteWithinMonths = completeWithinMonths,
                ValidityMonths = validityMonths
            };
        }
    }
}
