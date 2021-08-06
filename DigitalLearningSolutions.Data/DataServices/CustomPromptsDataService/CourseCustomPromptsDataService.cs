namespace DigitalLearningSolutions.Data.DataServices.CustomPromptsDataService
{
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public partial class CustomPromptsDataService
    {
        public CourseCustomPromptsResult GetCourseCustomPrompts(int customisationId, int centreId, int categoryId)
        {
            var result = connection.Query<CourseCustomPromptsResult>(
                @"SELECT
                        cp1.CoursePrompt AS CustomField1Prompt,
                        cu.Q1Options AS CustomField1Options,
                        cu.Q1Mandatory AS CustomField1Mandatory,
                        cp2.CoursePrompt AS CustomField2Prompt,
                        cu.Q2Options AS CustomField2Options,
                        cu.Q2Mandatory AS CustomField2Mandatory,
                        cp3.CoursePrompt AS CustomField3Prompt,
                        cu.Q3Options AS CustomField3Options,
                        cu.Q3Mandatory AS CustomField3Mandatory,
                        ap.CourseCategoryID
                    FROM
                        Customisations AS cu
                    LEFT JOIN CoursePrompts AS cp1
                        ON cu.CourseField1PromptID = cp1.CoursePromptID
                    LEFT JOIN CoursePrompts AS cp2
                        ON cu.CourseField2PromptID = cp2.CoursePromptID
                    LEFT JOIN CoursePrompts AS cp3
                        ON cu.CourseField3PromptID = cp3.CoursePromptID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    WHERE cu.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL
                        AND cu.CustomisationID = @customisationId",
                new { customisationId, centreId, categoryId }
            ).Single();

            return result;
        }

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, bool mandatory, string? options)
        {
            connection.Execute(
                @$"UPDATE Customisations
                    SET
                        Q{promptNumber}Mandatory = @mandatory,
                        Q{promptNumber}Options = @options
                    WHERE CustomisationID = @customisationId",
                new { mandatory, options, customisationId }
            );
        }

        public void UpdateCustomPromptForCourse(
            int customisationId,
            int promptNumber,
            int promptId,
            bool mandatory,
            string? options
        )
        {
            connection.Execute(
                @$"UPDATE Customisations
                    SET
                        CourseField{promptNumber}PromptID = @promptId,
                        Q{promptNumber}Mandatory = @mandatory,
                        Q{promptNumber}Options = @options
                    WHERE CustomisationID = @customisationId",
                new { promptId, mandatory, options, customisationId }
            );
        }

        public string GetPromptNameForCustomisationAndPromptNumber(int customisationId, int promptNumber)
        {
            return connection.Query<string>(
                @$"SELECT
                        cp.CustomPrompt
                    FROM Customisations c
                    LEFT JOIN CustomPrompts cp
                        ON c.CourseField{promptNumber}PromptID = cp.CustomPromptID
                    WHERE CustomisationID = @customisationId",
                new { customisationId }
            ).Single();
        }
    }
}
