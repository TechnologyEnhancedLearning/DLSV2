namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Extensions.Logging;

    public interface ISectionContentService
    {
        SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId);
    }

    public class SectionContentService : ISectionContentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SectionContentService> logger;

        public SectionContentService(IDbConnection connection, ILogger<SectionContentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId)
        {
            return connection.QueryFirstOrDefault<SectionContent>(
                @"
                    SELECT
                        Customisations.CustomisationName,
                        Applications.ApplicationName,
                        Sections.SectionName,
                        COALESCE(SUM(aspProgress.TutTime), 0) AS TimeMins, 
                        Sections.AverageSectionMins AS AverageSectionTime, 
                        dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                        (CASE
                            WHEN Progress.CandidateID IS NULL
                                OR dbo.CheckCustomisationSectionHasLearning(Progress.CustomisationID, Sections.SectionID) = 0
                            THEN 0
                            ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
                        END) AS PercentComplete
                    FROM
                        Tutorials
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                        INNER JOIN Customisations
                            ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Customisations.ApplicationID
                        INNER JOIN Sections
                            ON Sections.SectionID = Tutorials.SectionID
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = Customisations.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN aspProgress
                            ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                            and aspProgress.ProgressID = Progress.ProgressID
                    WHERE
                        CustomisationTutorials.CustomisationID = @customisationId
	                    AND Sections.SectionID = @sectionId
	                    AND (Sections.ArchivedDate IS NULL)
	                    AND (CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1 OR CustomisationTutorials.Status = 1)
                    GROUP BY
                        Sections.SectionID,
                        Customisations.CustomisationID,
                        Progress.ProgressID,
                        Progress.CustomisationID,
                        Progress.CandidateID,
                        Customisations.CustomisationName,
                        Applications.ApplicationName,
                        Sections.SectionName, 
                        Sections.AverageSectionMins",
                new { customisationId, candidateId, sectionId }
            );
        }
    }
}
