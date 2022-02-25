namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICentreRegistrationPromptsDataService
    {
        public CentreRegistrationPromptsResult GetCentreRegistrationPromptsByCentreId(int centreId);
        public void UpdateCentreRegistrationPrompt(int centreId, int promptNumber, bool mandatory, string? options);

        public IEnumerable<(int, string)> GetCustomPromptsAlphabetical();

        public void UpdateCentreRegistrationPrompt(
            int centreId,
            int promptNumber,
            int promptId,
            bool mandatory,
            string? options
        );

        public string GetCentreRegistrationPromptNameAndPromptNumber(int centreId, int promptNumber);
    }

    public class CentreRegistrationPromptsDataService : ICentreRegistrationPromptsDataService
    {
        private readonly IDbConnection connection;

        public CentreRegistrationPromptsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CentreRegistrationPromptsResult GetCentreRegistrationPromptsByCentreId(int centreId)
        {
            var result = connection.Query<CentreRegistrationPromptsResult>(
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

        public void UpdateCentreRegistrationPrompt(int centreId, int promptNumber, bool mandatory, string? options)
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

        public void UpdateCentreRegistrationPrompt(
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

        public string GetCentreRegistrationPromptNameAndPromptNumber(int centreId, int promptNumber)
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
    }
}
