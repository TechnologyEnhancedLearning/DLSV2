UPDATE Centres SET BetaTesting = 0 WHERE CentreID = 101
GO
UPDATE Candidates SET EmailAddress = 'kevin.whittaker1@nhs.net', FirstName = 'xxxx', LastName  = 'xxxxxx', Password = 'ABy0903vXLKIV6/PLI/Z300XEDuDlqboqkSHwxrL0n0QhQ3uk3oZxAcshtmUOzXZHg==' WHERE CandidateID = 3
GO
UPDATE Candidates SET Password = 'ADyLcAmuAkPwMkZW+ivvk/GCq/0yn0m08eP2hIPPvjKJwmvj6pBfmDrTv16tMz8xww==' WHERE CandidateID = 254480
GO
INSERT INTO CandidateAssessments (CandidateID, SelfAssessmentID, StartedDate) VALUES (11,1,'2020-09-01 14:10:37.447')
GO
UPDATE SelfAssessments SET Vocabulary = 'Capability', LinearNavigation = 1 WHERE ID = 1
GO
UPDATE [AssessmentQuestions] SET AssessmentQuestionInputTypeID = 2 WHERE ID IN(1,2)
GO
UPDATE Applications SET IncludeCertification = 1 WHERE ApplicationID = 308
GO
INSERT INTO [dbo].[CentreSelfAssessments]
           ([CentreID]
           ,[SelfAssessmentID]
           ,[AllowEnrolment])
     VALUES
           (101
           ,1
           ,1)
GO


