UPDATE [dbo].[Customisations]
    SET
        [CourseField1PromptID] = 1,
		[Q1Options] = 'Test',
        [CourseField2PromptID] = 2
    WHERE
        [CustomisationID] = 100;

UPDATE [dbo].[Progress]
    SET
        [Answer1] = 'Test'
    WHERE
        [CustomisationID] = 100 AND [CandidateID] = 1;
