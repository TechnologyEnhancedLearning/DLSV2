namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using Microsoft.Data.SqlClient;

    public class DiagnosticAssessmentTestHelper
    {
        private SqlConnection connection;

        public DiagnosticAssessmentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void UpdateDiagAssessOutOf(int tutorialId, int diagAssessOutOf)
        {
            connection.Execute(
                @"UPDATE Tutorials
                     SET DiagAssessOutOf = @diagAssessOutOf

                   WHERE Tutorials.TutorialID = @tutorialId;",
                new { tutorialId, diagAssessOutOf }
            );
        }
    }
}
