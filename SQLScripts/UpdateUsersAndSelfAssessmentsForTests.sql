UPDATE Candidates SET EmailAddress = 'kevin.whittaker1@nhs.net', FirstName = 'Kevin', LastName  = 'Whittaker', Password = 'ABy0903vXLKIV6/PLI/Z300XEDuDlqboqkSHwxrL0n0QhQ3uk3oZxAcshtmUOzXZHg==' WHERE CandidateID = 3
GO
UPDATE Candidates SET Password = 'ADyLcAmuAkPwMkZW+ivvk/GCq/0yn0m08eP2hIPPvjKJwmvj6pBfmDrTv16tMz8xww==' WHERE CandidateID = 254480
GO
UPDATE SelfAssessments SET Vocabulary = 'Capability', LinearNavigation = 1 WHERE ID = 1
GO
UPDATE [AssessmentQuestions] SET AssessmentQuestionInputTypeID = 2 WHERE ID IN(1,2)
GO
UPDATE [CandidateAssessments] SET UserBookmark = '/LearningPortal/SelfAssessment/1/Capabilities' WHERE ID = 1
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


