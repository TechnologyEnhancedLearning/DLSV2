namespace DigitalLearningSolutions.Data.Services.CustomPromptsService
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.CustomPromptsDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;

    public interface ICustomPromptsService
    {
        public CentreCustomPrompts GetCustomPromptsForCentreByCentreId(int centreId);

        public CentreCustomPromptsWithAnswers? GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(
            int centreId,
            DelegateUser? delegateUser
        );

        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>
            GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(
                int centreId,
                IEnumerable<DelegateUser> delegateUsers
            );

        public void UpdateCustomPromptForCentre(int centreId, int promptNumber, bool mandatory, string? options);

        List<(int id, string value)> GetCustomPromptsAlphabeticalList();

        public bool AddCustomPromptToCentre(int centreId, int promptId, bool mandatory, string? options);

        public void RemoveCustomPromptFromCentre(int centreId, int promptNumber);

        public string GetPromptNameForCentreAndPromptNumber(int centreId, int promptNumber);

        public CourseAdminFields? GetCustomPromptsForCourse(int customisationId, int centreId, int categoryId);

        public List<CustomPromptWithAnswer> GetCustomPromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId,
            int centreId,
            int categoryId = 0
        );

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, bool mandatory, string? options);

        public void RemoveCustomPromptFromCourse(int customisationId, int promptNumber);

        public string GetPromptNameForCourseAndPromptNumber(int customisationId, int promptNumber);
    }

    public partial class CustomPromptsService : ICustomPromptsService
    {
        private readonly ICustomPromptsDataService customPromptsDataService;
        private readonly ILogger<CustomPromptsService> logger;
        private readonly IUserDataService userDataService;

        public CustomPromptsService(
            ICustomPromptsDataService customPromptsDataService,
            ILogger<CustomPromptsService> logger,
            IUserDataService userDataService
        )
        {
            this.customPromptsDataService = customPromptsDataService;
            this.logger = logger;
            this.userDataService = userDataService;
        }

        private static CustomPrompt? PopulateCustomPrompt(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CustomPrompt(promptNumber, prompt, options, mandatory) : null;
        }

        private static CustomPromptWithAnswer? PopulateCustomPromptWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null ? new CustomPromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }
    }
}
