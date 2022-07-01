namespace DigitalLearningSolutions.Data.Enums
{
    public enum SelfAssessmentCompetencyFilter
    {
        RequiresSelfAssessment = -8,
        SelfAssessed = -7,
        ConfirmationRequested = -6,
        Verified = -5, /* Confirmed */
        ConfirmationRejected = -4,
        MeetingRequirements = -3,
        PartiallyMeetingRequirements = -2,
        NotMeetingRequirements = -1
    }
}
