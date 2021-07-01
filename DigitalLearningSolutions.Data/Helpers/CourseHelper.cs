namespace DigitalLearningSolutions.Data.Helpers
{
    public class CourseHelper
    {
        public const string DelegateCount =
            @"(SELECT COUNT(CandidateID)
                FROM dbo.Progress AS pr
                WHERE pr.CustomisationID = cu.CustomisationID) AS DelegateCount";

        public const string CompletedCount =
            @"(SELECT COUNT(CandidateID)
                FROM dbo.Progress AS pr
                WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL) AS CompletedCount";

        public const string AllAttempts =
            @"(SELECT COUNT(AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL) AS AllAttempts";

        public const string AttemptsPassed =
            @"(SELECT COUNT(AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1) AS AttemptsPassed";
    }
}
