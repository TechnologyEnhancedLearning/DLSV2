﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface ITutorialContentService
    {
        TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        );
        TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId);
        TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId);
    }

    public class TutorialContentService : ITutorialContentService
    {
        private readonly IDbConnection connection;

        public TutorialContentService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        )
        {
            return connection.QueryFirstOrDefault<TutorialInformation>(
                @"SELECT Tutorials.TutorialID AS Id,
                         Tutorials.TutorialName AS Name,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         COALESCE(TutStatus.Status, 'Not started') AS Status,
                         COALESCE(aspProgress.TutTime, 0) AS TimeSpent,
                         Tutorials.AverageTutMins AS AverageTutorialDuration,
                         COALESCE(aspProgress.DiagLast, 0) AS CurrentScore,
                         Tutorials.DiagAssessOutOf AS PossibleScore,
                         CustomisationTutorials.DiagStatus AS CanShowDiagnosticStatus,
                         COALESCE(aspProgress.DiagAttempts, 0) AS AttemptCount,
                         Tutorials.Objectives,
                         Tutorials.VideoPath,
                         Tutorials.TutorialPath,
                         Tutorials.SupportingMatsPath AS SupportingMaterialPath,
                         Sections.PLAssessPath AS PostLearningAssessmentPath,
                         MIN(SubsequentTutorials.TutorialID) AS NextTutorialID,
                         MIN(SubsequentSections.SectionID) AS NextSectionID
                    FROM Tutorials
                         INNER JOIN CustomisationTutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationId = Applications.ApplicationId

                         LEFT JOIN CustomisationTutorials AS SubsequentCustomisationTutorials
                         ON SubsequentCustomisationTutorials.CustomisationID = Customisations.CustomisationID
                            AND SubsequentCustomisationTutorials.Status = 1

                         LEFT JOIN Tutorials AS SubsequentTutorials
                         ON SubsequentTutorials.SectionID = Sections.SectionID
                            AND Tutorials.OrderByNumber < SubsequentTutorials.OrderByNumber
                            AND SubsequentCustomisationTutorials.TutorialID = SubsequentTutorials.TutorialID

                         LEFT JOIN Tutorials AS SubsequentSectionsTutorials
                         ON  SubsequentCustomisationTutorials.TutorialID = SubsequentSectionsTutorials.TutorialID

                         LEFT JOIN Sections AS SubsequentSections
                         ON SubsequentSectionsTutorials.SectionID = SubsequentSections.SectionID
                            AND Sections.SectionNumber < SubsequentSections.SectionNumber

                         LEFT JOIN Progress
                         ON CustomisationTutorials.CustomisationID = Progress.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0

                         LEFT JOIN aspProgress
                         ON aspProgress.TutorialID = Tutorials.TutorialID
                            AND aspProgress.ProgressID = Progress.ProgressID

                         LEFT JOIN TutStatus
                         ON aspProgress.TutStat = TutStatus.TutStatusID
                   WHERE CustomisationTutorials.CustomisationID = @customisationId
                     AND Tutorials.SectionId = @sectionId
                     AND Tutorials.TutorialID = @tutorialId
                     AND Customisations.Active = 1
                     AND CustomisationTutorials.Status = 1
                   GROUP BY Tutorials.TutorialID,
                            Tutorials.TutorialName,
                            Applications.ApplicationName,
                            Customisations.CustomisationName,
                            TutStatus.Status,
                            aspProgress.TutTime,
                            Tutorials.AverageTutMins,
                            aspProgress.DiagLast,
                            Tutorials.DiagAssessOutOf,
                            CustomisationTutorials.DiagStatus,
                            aspProgress.DiagAttempts,
                            Tutorials.Objectives,
                            Tutorials.VideoPath,
                            Tutorials.TutorialPath,
                            Tutorials.SupportingMatsPath,
                            Sections.PLAssessPath;",
            new { candidateId, customisationId, sectionId, tutorialId });
        }

        public TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId)
        {
            return connection.QueryFirstOrDefault<TutorialContent>(
                @"SELECT Tutorials.TutorialName,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Tutorials.TutorialPath,
                         Customisations.CurrentVersion
                    FROM CustomisationTutorials
                         INNER JOIN Tutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID
                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.SectionID = @sectionId
                         AND Tutorials.TutorialId = @tutorialId
                         AND Customisations.Active = 1
                         AND CustomisationTutorials.Status = 1;",
                new { customisationId, sectionId, tutorialId });
        }

        public TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId)
        {
            try
            {
                return connection.QueryFirstOrDefault<TutorialVideo>(
                    @"SELECT Tutorials.TutorialName,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Tutorials.VideoPath
                    FROM CustomisationTutorials
                         INNER JOIN Tutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID
                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.SectionID = @sectionId
                         AND Tutorials.TutorialId = @tutorialId
                         AND Customisations.Active = 1
                         AND CustomisationTutorials.Status = 1;",
                    new { customisationId, sectionId, tutorialId });
            }
            catch (DataException e)
            {
                if (e.InnerException is VideoNotFoundException)
                {
                    return null;
                }
                else throw;
            }
        }
    }
}
