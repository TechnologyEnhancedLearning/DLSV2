namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
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
    }

    public class CustomPromptsDataService : ICustomPromptsDataService
    {
        private readonly IDbConnection connection;

        public CustomPromptsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CentreCustomPromptsResult GetCentreCustomPromptsByCentreId(int centreId)
        {
            var result = connection.Query<CentreCustomPromptsResult>(
                @"SELECT 
	                    c.CentreID, 
	                    cp1.CustomPrompt AS CustomField1Prompt,
	                    c.F1Options AS CustomField1Options, 
	                    c.F1Mandatory AS CustomField1Mandatory, 
	                    cp2.CustomPrompt AS CustomField2Prompt,
	                    c.F2Options AS CustomField2Options, 
	                    c.F2Mandatory AS CustomField2Mandatory,
	                    cp3.CustomPrompt AS CustomField3Prompt,
	                    c.F3Options AS CustomField3Options, 
	                    c.F3Mandatory AS CustomField3Mandatory,
	                    cp4.CustomPrompt AS CustomField4Prompt,
	                    c.F4Options AS CustomField4Options, 
	                    c.F4Mandatory AS CustomField4Mandatory,
	                    cp5.CustomPrompt AS CustomField5Prompt,
	                    c.F5Options AS CustomField5Options, 
	                    c.F5Mandatory AS CustomField5Mandatory,
	                    cp6.CustomPrompt AS CustomField6Prompt,
	                    c.F6Options AS CustomField6Options, 
	                    c.F6Mandatory AS CustomField6Mandatory
                    FROM 
	                    Centres c
                    LEFT JOIN CustomPrompts cp1 
	                    ON c.CustomField1PromptID = cp1.CustomPromptID
                    LEFT JOIN CustomPrompts cp2 
	                    ON c.CustomField2PromptID = cp2.CustomPromptID
                    LEFT JOIN CustomPrompts cp3 
	                    ON c.CustomField3PromptID = cp3.CustomPromptID
                    LEFT JOIN CustomPrompts cp4 
	                    ON c.CustomField4PromptID = cp4.CustomPromptID
                    LEFT JOIN CustomPrompts cp5 
	                    ON c.CustomField5PromptID = cp5.CustomPromptID
                    LEFT JOIN CustomPrompts cp6 
	                    ON c.CustomField6PromptID = cp6.CustomPromptID
                    where CentreID = @centreId",
                new { centreId }
            ).Single();

            return result;
        }

        public void UpdateCustomPromptForCentre(int centreId, int promptNumber, bool mandatory, string? options)
        {
            connection.Execute(
                @$"UPDATE Centres
                    SET
                        F{promptNumber}Mandatory = @mandatory,
                        F{promptNumber}Options = @options
                    WHERE CentreID = @centreId",
                new { mandatory, options, centreId }
            );
        }

        public IEnumerable<(int, string)> GetCustomPromptsAlphabetical()
        {
            var jobGroups = connection.Query<(int, string)>
            (
                @"SELECT CustomPromptID, CustomPrompt
                        FROM CustomPrompts
                        WHERE Active = 1
                        ORDER BY CustomPrompt"
            );
            return jobGroups;
        }

        public void UpdateCustomPromptForCentre(
            int centreId,
            int promptNumber,
            int promptId,
            bool mandatory,
            string? options
        )
        {
            connection.Execute(
                @$"UPDATE Centres
                    SET
                        CustomField{promptNumber}PromptId = @promptId,
                        F{promptNumber}Mandatory = @mandatory,
                        F{promptNumber}Options = @options
                    WHERE CentreID = @centreId",
                new { promptId, mandatory, options, centreId }
            );
        }

        public string GetPromptNameForCentreAndPromptNumber(int centreId, int promptNumber)
        {
            return connection.Query<string>(
                @$"SELECT
                        cp.CustomPrompt  
                    FROM Centres c
                    LEFT JOIN CustomPrompts cp
                        ON c.CustomField{promptNumber}PromptID = cp.CustomPromptID
                    WHERE CentreID = @centreId",
                new { centreId }
            ).Single();
        }

        public CourseCustomPromptsResult? GetCourseCustomPrompts(int customisationId, int centreId, int categoryId)
        {
            var result = connection.Query<CourseCustomPromptsResult>(
                @"SELECT
                        cu.CustomisationID,
	                    cu.CentreID, 
	                    cp1.CoursePrompt AS CustomField1Prompt,
	                    cu.Q1Options AS CustomField1Options, 
	                    cu.Q1Mandatory AS CustomField1Mandatory, 
	                    cp2.CoursePrompt AS CustomField2Prompt,
	                    cu.Q2Options AS CustomField2Options, 
	                    cu.Q2Mandatory AS CustomField2Mandatory,
	                    cp3.CoursePrompt AS CustomField3Prompt,
	                    cu.Q3Options AS CustomField3Options, 
	                    cu.Q3Mandatory AS CustomField3Mandatory
                    FROM 
	                    Customisations AS cu
                    LEFT JOIN CoursePrompts AS cp1 
	                    ON cu.CourseField1PromptID = cp1.CoursePromptID
                    LEFT JOIN CoursePrompts AS cp2 
	                    ON cu.CourseField2PromptID = cp2.CoursePromptID
                    LEFT JOIN CoursePrompts AS cp3 
	                    ON cu.CourseField3PromptID = cp3.CoursePromptID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId = 0) 
                        AND cu.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL
                        AND cu.CustomisationID = @customisationId",
                new { customisationId, centreId, categoryId }
            ).SingleOrDefault();

            return result;
        }
    }
}
