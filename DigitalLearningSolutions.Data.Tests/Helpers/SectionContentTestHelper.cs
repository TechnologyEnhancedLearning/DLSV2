namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Data.SqlClient;

    public class SectionContentTestHelper
    {
        private SqlConnection connection;

        public SectionContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void UpdateSectionNumber(int sectionId, int sectionNumber)
        {
            connection.Execute(
                @"UPDATE Sections
                        SET SectionNumber = @sectionNumber
                        WHERE SectionID = @sectionId",
                new { sectionId, sectionNumber }
            );
        }

        public void UpdateDiagnosticAttempts(int tutorialId, int progressId, int diagAttempts)
        {
            connection.Execute(
                @"UPDATE aspProgress
                        SET DiagAttempts = @diagAttempts
                        WHERE TutorialID = @tutorialId AND ProgressID = @progressId",
                new { tutorialId, progressId, diagAttempts }
            );
        }

        public void UpdateDiagnosticScore(int tutorialId, int progressId, int score)
        {
            connection.Execute(
                @"UPDATE aspProgress
                        SET DiagLast = @score
                        WHERE TutorialID = @tutorialId AND ProgressID = @progressId",
                new { tutorialId, progressId, score }
            );
        }
    }
}
