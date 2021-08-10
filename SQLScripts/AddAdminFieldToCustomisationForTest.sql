UPDATE [mbdbx101_test].[dbo].[Customisations]
    SET
        [CourseField1PromptID] = 1,
		[Q1Options] = 'Test'
    WHERE
        [CustomisationID] = 100;

UPDATE [mbdbx101_test].[dbo].[Progress]
    SET
        [Answer1] = 'Test'
    WHERE
        [CustomisationID] = 100 AND [CandidateID] = 1;
