namespace DigitalLearningSolutions.Data.Constants
{
    /// <summary>
    /// Result codes returned when adding a collaborator (contributor or reviewer)
    /// to a framework or self assessment. Positive values are the new collaborator ID.
    /// </summary>
    public static class CollaboratorResultCodes
    {
        public const int InsertFailed = -1;
        public const int AlreadyCollaborator = -2;
        public const int EmailMissing = -3;
        public const int NoActiveAdminAccount = -4;
        public const int IsOwner = -5;
        public const int AdminAccountSelectionRequired = -6;
    }
}
