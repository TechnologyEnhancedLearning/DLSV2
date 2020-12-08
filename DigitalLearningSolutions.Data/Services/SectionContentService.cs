﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Extensions.Logging;

    public interface ISectionContentService
    {
        SectionContent? GetSectionContent(int customisationId, int progressId, int sectionId);
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

        public SectionContent? GetSectionContent(int customisationId, int progressId, int sectionId)
        {
            return connection.QueryFirstOrDefault<SectionContent>(
                @"
                    SELECT
	                    Customisations.CustomisationName,
	                    Applications.ApplicationName,
	                    Sections.SectionName, 
	                    SUM(aspProgress.TutTime) AS TimeMins, 
	                    Sections.AverageSectionMins AS AverageSectionTime, 
                        dbo.CheckCustomisationSectionHasLearning(Progress.CustomisationID, Sections.SectionID) AS HasLearning,
	                    (CASE
		                    WHEN Progress.CandidateID IS NULL
				                    OR dbo.CheckCustomisationSectionHasLearning(Progress.CustomisationID, Sections.SectionID) = 0
		                    THEN 0
		                    ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
	                    END) AS PercentComplete
                    FROM aspProgress 
	                    INNER JOIN Progress ON aspProgress.ProgressID = Progress.ProgressID 
	                    INNER JOIN Tutorials ON Tutorials.TutorialID = aspProgress.TutorialID
	                    INNER JOIN CustomisationTutorials ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
	                    INNER JOIN Sections ON Sections.SectionID = Tutorials.SectionID 
	                    INNER JOIN Customisations ON Progress.CustomisationID = Customisations.CustomisationID
	                    INNER JOIN Applications ON Customisations.ApplicationID = Applications.ApplicationID
	                    LEFT OUTER JOIN AssessAttempts ON Progress.ProgressID = AssessAttempts.ProgressID AND Sections.SectionNumber = AssessAttempts.SectionNumber
                    WHERE (CustomisationTutorials.CustomisationID = Progress.CustomisationID)
	                    AND (Progress.CustomisationID = @customisationId)
	                    AND (Progress.ProgressID = @progressId) 
	                    AND (Sections.SectionID = @sectionId)
                    GROUP BY Sections.SectionID, 
	                    Sections.SectionName, 
	                    Sections.AverageSectionMins, 
	                    Progress.CandidateID, 
	                    Progress.CustomisationID,
	                    Customisations.CustomisationName,
	                    Applications.ApplicationName;",
                new { customisationId, progressId, sectionId }
            );
        }
    }
}
