UPDATE [mbdbx101_test].[dbo].[Customisations]
    SET
        [CourseField1PromptID] = 1,
		[Q1Options] = 'Test',
		[CourseField2PromptID] = 2
    WHERE
        [CustomisationID] = 100;
