namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Constants;

    /// <summary>
    /// Maps the result codes returned when adding a collaborator to user-facing error messages.
    /// </summary>
    public static class CollaboratorResultMessageHelper
    {
        public static string GetErrorMessage(int resultCode, string entityDescription)
        {
            return resultCode switch
            {
                CollaboratorResultCodes.EmailMissing => "Email address should not be empty",
                CollaboratorResultCodes.AlreadyCollaborator => "A user with the email address has been previously added",
                CollaboratorResultCodes.NoActiveAdminAccount => "The email address must match a registered DLS Admin account",
                CollaboratorResultCodes.IsOwner => $"The owner cannot be the collaborator of the {entityDescription}.",
                CollaboratorResultCodes.AdminAccountSelectionRequired => "This email address has more than one admin account. Select which account to add.",
                _ => "User not added, kindly try again.",
            };
        }
    }
}
