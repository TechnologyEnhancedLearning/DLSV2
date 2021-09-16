namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseAdminFieldsDataService
    {
        public CourseAdminFieldsResult? GetCourseAdminFields(int customisationId, int centreId, int categoryId);

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, string? options);

        public IEnumerable<(int, string)> GetCoursePromptsAlphabetical();

        public void UpdateCustomPromptForCourse(
            int customisationId,
            int promptNumber,
            int promptId,
            string? options
        );

        public string GetPromptName(int customisationId, int promptNumber);

        public int GetAnswerCountForCourseAdminField(int customisationId, int promptNumber);

        public void DeleteAllAnswersForCourseAdminField(int customisationId, int promptNumber);
    }

    public class CourseAdminFieldsDataService : ICourseAdminFieldsDataService
    {
        private readonly IDbConnection connection;

        public CourseAdminFieldsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CourseAdminFieldsResult GetCourseAdminFields(int customisationId, int centreId, int categoryId)
        {
            var result = connection.Query<CourseAdminFieldsResult>(
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

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, string? options)
        {
            connection.Execute(
                @$"UPDATE Customisations
                    SET
                        Q{promptNumber}Options = @options
                    WHERE CustomisationID = @customisationId",
                new { options, customisationId }
            );
        }

        public IEnumerable<(int, string)> GetCoursePromptsAlphabetical()
        {
            var coursePrompts = connection.Query<(int, string)>
            (
                @"SELECT CoursePromptID, CoursePrompt
                        FROM CoursePrompts
                        WHERE Active = 1
                        ORDER BY CoursePrompt"
            );
            return coursePrompts;
        }

        public void UpdateCustomPromptForCourse(
            int customisationId,
            int promptNumber,
            int promptId,
            string? options
        )
        {
            connection.Execute(
                @$"UPDATE Customisations
                    SET
                        CourseField{promptNumber}PromptID = @promptId,
                        Q{promptNumber}Options = @options
                    WHERE CustomisationID = @customisationId",
                new { promptId, options, customisationId }
            );
        }

        public string GetPromptName(int customisationId, int promptNumber)
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

        public int GetAnswerCountForCourseAdminField(int customisationId, int promptNumber)
        {
            return connection.Query<string>(
                $@"SELECT Answer{promptNumber}
                        FROM Progress
                        WHERE CustomisationID = @customisationId AND Answer{promptNumber} IS NOT NULL",
                new { customisationId }
            ).Count(x => !string.IsNullOrWhiteSpace(x));
        }

        public void DeleteAllAnswersForCourseAdminField(int customisationId, int promptNumber)
        {
            connection.Execute(
                $@"UPDATE Progress
                        SET Answer{promptNumber} = NULL
                        WHERE CustomisationID = @customisationId",
                new { customisationId }
            );
        }
    }
}
