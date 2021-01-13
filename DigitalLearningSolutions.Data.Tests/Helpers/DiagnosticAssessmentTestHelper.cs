﻿namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
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

        public static DiagnosticAssessment CreateDefaultDiagnosticAssessment(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            int diagnosticAttempts = 1,
            int sectionScore = 2,
            int maxSectionScore = 3,
            string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DIAGNEW/itspplayer.html",
            bool canSelectTutorials = true
        )
        {
            return new DiagnosticAssessment(
                applicationName,
                customisationName,
                sectionName,
                diagnosticAttempts,
                sectionScore,
                maxSectionScore,
                diagnosticAssessmentPath,
                canSelectTutorials
            );
        }

        public static DiagnosticContent CreateDefaultDiagnosticContent(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DIAGNEW/itspplayer.html",
            bool canSelectTutorials = true,
            int postLearningPassThreshold = 50,
            int currentVersion = 1
        )
        {
            return new DiagnosticContent(
                applicationName,
                customisationName,
                sectionName,
                diagnosticAssessmentPath,
                canSelectTutorials,
                postLearningPassThreshold,
                currentVersion
            );
        }
    }
}
