namespace DigitalLearningSolutions.Web.IntegrationTests.TestHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Data.Models.User;
    using FakeItEasy;

    public static class TestUserDataService
    {
        private static readonly List<UserAccount> UserAccountData = new List<UserAccount>
        {
            new UserAccount
            {
                Id = 1,
                PrimaryEmail = "test_user_1_@testdls.",
                FirstName = "Fxxxxxxxx1",
                LastName = "Lxxxxxxx1",
                LearningHubAuthId = 1,
            },
            new UserAccount
            {
                Id = 2,
                PrimaryEmail = "test_user_2_@testdls.",
                FirstName = "Fxxxxxxxx2",
                LastName = "Lxxxxxxx2",
                LearningHubAuthId = null,
            },
        };

        private static readonly List<TestUserDataModel> UserData = new List<TestUserDataModel>
        {
            new TestUserDataModel
            {
                Id = 1,
                UserId = 1,
                CentreId = 1,
                CandidateNumber = "TST1",
                CentreName = "TestCenter 101",
                CentreActive = true,
                Approved = true,
                Active = true,
                SessionData = new Dictionary<string, string>
                {
                    { LinkLearningHubRequest.SessionIdentifierKey, "635FB79F-E56C-4BB0-966B-A027E3660BBC" },
                },
            },
            new TestUserDataModel
            {
                Id = 2,
                UserId = 2,
                CentreId = 1,
                CandidateNumber = "TST2",
                CentreName = "TestCenter 1",
                CentreActive = true,
                Approved = true,
                Active = true,
                SessionData = new Dictionary<string, string>
                {
                    { LinkLearningHubRequest.SessionIdentifierKey, "7D297B5B-9C63-4A95-94D4-176582005DA9" },
                },
            },
        };

        public static UserAccount GetUserAccount(int delegateId)
        {
            return UserAccountData.First(u => u.Id == delegateId);
        }

        public static TestUserDataModel GetDelegate(int delegateId)
        {
            return UserData.First(u => u.Id == delegateId);
        }

        public static int? GetAuthId(int delegateId)
        {
            return UserAccountData.First(u => u.Id == delegateId).LearningHubAuthId;
        }

        public static void SetAuthId(int delegateId, int authId)
        {
            var user = UserAccountData.First(u => u.Id == delegateId);
            user.LearningHubAuthId = authId;
        }

        public static IUserDataService FakeUserDataService()
        {
            var fakeUserDataService = A.Fake<IUserDataService>();

            A.CallTo(() => fakeUserDataService.GetDelegateUserLearningHubAuthId(A<int>._))
                .ReturnsLazily((int delegateId) => GetAuthId(delegateId));

            A.CallTo(() => fakeUserDataService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                .Invokes((int delegateId, int authId) => SetAuthId(delegateId, authId))
                .DoesNothing();

            return fakeUserDataService;
        }
    }
}
