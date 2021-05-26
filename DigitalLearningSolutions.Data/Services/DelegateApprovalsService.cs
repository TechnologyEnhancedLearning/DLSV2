namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IDelegateApprovalsService
    {
        public List<(DelegateUser, List<CustomPromptWithAnswer>)>
            GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(int centreId);
    }

    public class DelegateApprovalsService : IDelegateApprovalsService
    {
        private readonly IUserDataService userDataService;
        private readonly ICustomPromptsService customPromptsService;

        public DelegateApprovalsService(IUserDataService userDataService, ICustomPromptsService customPromptsService)
        {
            this.userDataService = userDataService;
            this.customPromptsService = customPromptsService;
        }

        public List<(DelegateUser, List<CustomPromptWithAnswer>)> GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(int centreId)
        {
            var users = userDataService.GetUnapprovedDelegateUsersByCentreId(centreId);
            var usersWithPrompts =
                customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(centreId, users);

            return usersWithPrompts;
        }
    }
}
