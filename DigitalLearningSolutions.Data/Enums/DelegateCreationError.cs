namespace DigitalLearningSolutions.Data.Enums
{
    public class DelegateCreationError : Enumeration
    {
        public static DelegateCreationError UnexpectedError = new DelegateCreationError(
            1,
            nameof(UnexpectedError)
        );

        public static DelegateCreationError EmailAlreadyInUse = new DelegateCreationError(
            2,
            nameof(EmailAlreadyInUse)
        );

        public static DelegateCreationError ActiveAccountAlreadyExists = new DelegateCreationError(
            3,
            nameof(ActiveAccountAlreadyExists)
        );

        private DelegateCreationError(int id, string name) : base(id, name) { }
    }
}
