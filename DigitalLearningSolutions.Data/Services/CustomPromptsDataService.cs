namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICustomPromptsDataService
    {
        public CentreCustomPromptsResult GetCentreCustomPromptsByCentreId(int centreId);
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
    }
}
