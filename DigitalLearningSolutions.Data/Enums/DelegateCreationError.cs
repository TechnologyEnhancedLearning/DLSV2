namespace DigitalLearningSolutions.Data.Enums
{
    public class DelegateCreationError : Enumeration
    {
        public static DelegateCreationError UnexpectedError = new DelegateCreationError(
            1,
            nameof(UnexpectedError),
            "-1"
        );

        public static DelegateCreationError EmailAlreadyInUse = new DelegateCreationError(
            2,
            nameof(EmailAlreadyInUse),
            "-4"
        );

        private DelegateCreationError(int id, string name, string storedProcedureErrorCode) : base(id, name) { }
    }
}
