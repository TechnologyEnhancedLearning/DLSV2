namespace DigitalLearningSolutions.Data.DataServices.CustomPromptsDataService
{
    using System.Collections.Generic;
    using System.Data;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICustomPromptsDataService
    {
        public CentreCustomPromptsResult GetCentreCustomPromptsByCentreId(int centreId);
        public void UpdateCustomPromptForCentre(int centreId, int promptNumber, bool mandatory, string? options);

        public IEnumerable<(int, string)> GetCustomPromptsAlphabetical();

        public void UpdateCustomPromptForCentre(
            int centreId,
            int promptNumber,
            int promptId,
            bool mandatory,
            string? options
        );

        public string GetPromptNameForCentreAndPromptNumber(int centreId, int promptNumber);
        public CourseCustomPromptsResult? GetCourseCustomPrompts(int customisationId, int centreId, int categoryId);

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, bool mandatory, string? options);

        public void UpdateCustomPromptForCourse(
            int customisationId,
            int promptNumber,
            int promptId,
            bool mandatory,
            string? options
        );

        public string GetPromptNameForCustomisationAndPromptNumber(int customisationId, int promptNumber);
    }

    public partial class CustomPromptsDataService : ICustomPromptsDataService
    {
        private readonly IDbConnection connection;

        public CustomPromptsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }
    }
}
