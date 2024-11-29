namespace DigitalLearningSolutions.Data.Enums
{
    public enum SelfAssessmentCompetencyFilter
    {
        AwaitingConfirmation = -11,
        PendingConfirmation = -10,
        RequiresSelfAssessment = -9,
        SelfAssessed = -8,
        ConfirmationRequested = -7,
        Verified = -6, /* Confirmed */
        ConfirmationRejected = -5,
        Optional = -4,
        MeetingRequirements = -3,
        PartiallyMeetingRequirements = -2,
        NotMeetingRequirements = -1        
    }
}
