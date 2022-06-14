namespace DigitalLearningSolutions.Data.Enums
{
    public class AdminCreationError : Enumeration
    {
        public static AdminCreationError UnexpectedError = new AdminCreationError(
            1,
            nameof(UnexpectedError)
        );
        public static AdminCreationError EmailAlreadyInUse = new AdminCreationError(
            2,
            nameof(EmailAlreadyInUse)
        );// TODO HEEDLS-908 does this still need to exist?
        public static AdminCreationError ActiveAdminAlreadyExists = new AdminCreationError(
            3,
            nameof(ActiveAdminAlreadyExists)
        );

        public AdminCreationError(int id, string name) : base(id, name) { }
    }
}
