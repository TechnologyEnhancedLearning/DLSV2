namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
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
    }
}
