namespace DigitalLearningSolutions.Data.Enums
{
    public class AdminCreationError : Enumeration
    {
        public static AdminCreationError UnexpectedError = new AdminCreationError(
            1,
            nameof(UnexpectedError)
        );
        public static AdminCreationError AlreadyActiveAdminAtCentre = new AdminCreationError(
            2,
            nameof(AlreadyActiveAdminAtCentre)
        );

        public AdminCreationError(int id, string name) : base(id, name) { }
    }
}
