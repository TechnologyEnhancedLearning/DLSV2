namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseAdminFieldsDataService
    {
        CourseAdminFieldsResult GetCourseAdminFields(int customisationId);

        void UpdateAdminFieldForCourse(int customisationId, int promptNumber, string? options);

        IEnumerable<(int id, string name)> GetCoursePromptsAlphabetical();

        void UpdateAdminFieldForCourse(
            int customisationId,
            int promptNumber,
            int promptId,
            string? options
        );

        string GetPromptName(int customisationId, int promptNumber);

        int GetAnswerCountForCourseAdminField(int customisationId, int promptNumber);

        void DeleteAllAnswersForCourseAdminField(int customisationId, int promptNumber);

        IEnumerable<DelegateCourseAdminFieldAnswers> GetDelegateAnswersForCourseAdminFields(
            int customisationId,
            int centreId
        );

        int[] GetCourseFieldPromptIdsForCustomisation(int customisationId);
    }

    public class CourseAdminFieldsDataService : ICourseAdminFieldsDataService
    {
        private readonly IDbConnection connection;

        public CourseAdminFieldsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CourseAdminFieldsResult GetCourseAdminFields(int customisationId)
        {
            var result = connection.Query<CourseAdminFieldsResult>(
                @"SELECT
                        cp1.CoursePrompt AS CourseAdminField1Prompt,
                        cu.Q1Options AS CourseAdminField1Options,
                        cu.Q1Mandatory AS CentreRegistrationPrompt1Mandatory,
                        cp2.CoursePrompt AS CourseAdminField2Prompt,
                        cu.Q2Options AS CourseAdminField2Options,
                        cu.Q2Mandatory AS CentreRegistrationPrompt2Mandatory,
                        cp3.CoursePrompt AS CourseAdminField3Prompt,
                        cu.Q3Options AS CourseAdminField3Options,
                        cu.Q3Mandatory AS CentreRegistrationPrompt3Mandatory,
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
                    WHERE ap.ArchivedDate IS NULL
                        AND cu.CustomisationID = @customisationId",
                new { customisationId }
            ).Single();

            return result;
        }

        public void UpdateAdminFieldForCourse(int customisationId, int promptNumber, string? options)
        {
            connection.Execute(
                @$"UPDATE Customisations
                    SET
                        Q{promptNumber}Options = @options
                    WHERE CustomisationID = @customisationId",
                new { options, customisationId }
            );
        }

        public IEnumerable<(int id, string name)> GetCoursePromptsAlphabetical()
        {
            return connection.Query<(int, string)>
            (
                @"SELECT CoursePromptID, CoursePrompt
                        FROM CoursePrompts
                        WHERE Active = 1
                        ORDER BY CoursePrompt"
            );
        }

        public void UpdateAdminFieldForCourse(
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
                        cp.CoursePrompt
                    FROM Customisations c
                    LEFT JOIN CoursePrompts cp
                        ON c.CourseField{promptNumber}PromptID = cp.CoursePromptID
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

        public IEnumerable<DelegateCourseAdminFieldAnswers> GetDelegateAnswersForCourseAdminFields(
            int customisationId,
            int centreId
        )
        {
            return connection.Query<DelegateCourseAdminFieldAnswers>(
                @"SELECT
                        c.CandidateID AS DelegateId,
                        p.Answer1 AS Answer1,
                        p.Answer2 AS Answer2,
                        p.Answer3 AS Answer3
                    FROM Candidates AS c
                    INNER JOIN Progress AS p ON p.CandidateID = c.CandidateID
                    WHERE c.CentreID = @centreId
                        AND p.CustomisationID = @customisationId
                        AND RemovedDate IS NULL",
                new { customisationId, centreId }
            );
        }

        public int[] GetCourseFieldPromptIdsForCustomisation(
            int customisationId
        )
        {
            var result = connection.Query<(int, int, int)>(
                @"SELECT
                        CourseField1PromptID,
                        CourseField2PromptID,
                        CourseField3PromptID
                    FROM Customisations
                    WHERE CustomisationID = @customisationId",
                new { customisationId }
            ).Single();

            return new [] { result.Item1, result.Item2, result.Item3 };
        }
    }
}
