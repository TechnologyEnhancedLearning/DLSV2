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
                        cp1.CustomPromptID AS CentreRegistrationPrompt1Id,
	                    cp1.CustomPrompt AS CentreRegistrationPrompt1Prompt,
	                    c.F1Options AS CentreRegistrationPrompt1Options,
	                    c.F1Mandatory AS CentreRegistrationPrompt1Mandatory,
                        cp2.CustomPromptID AS CentreRegistrationPrompt2d,
	                    cp2.CustomPrompt AS CentreRegistrationPrompt2Prompt,
	                    c.F2Options AS CentreRegistrationPrompt2Options,
	                    c.F2Mandatory AS CentreRegistrationPrompt2Mandatory,
                        cp3.CustomPromptID AS CentreRegistrationPrompt3Id,
	                    cp3.CustomPrompt AS CentreRegistrationPrompt3Prompt,
	                    c.F3Options AS CentreRegistrationPrompt3Options,
	                    c.F3Mandatory AS CentreRegistrationPrompt3Mandatory,
                        cp4.CustomPromptID AS CentreRegistrationPrompt4Id,
	                    cp4.CustomPrompt AS CentreRegistrationPrompt4Prompt,
	                    c.F4Options AS CentreRegistrationPrompt4Options,
	                    c.F4Mandatory AS CentreRegistrationPrompt4Mandatory,
                        cp5.CustomPromptID AS CentreRegistrationPrompt5Id,
	                    cp5.CustomPrompt AS CentreRegistrationPrompt5Prompt,
	                    c.F5Options AS CentreRegistrationPrompt5Options,
	                    c.F5Mandatory AS CentreRegistrationPrompt5Mandatory,
                        cp6.CustomPromptID AS CentreRegistrationPrompt6Id,
	                    cp6.CustomPrompt AS CentreRegistrationPrompt6Prompt,
	                    c.F6Options AS CentreRegistrationPrompt6Options,
	                    c.F6Mandatory AS CentreRegistrationPrompt6Mandatory
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
