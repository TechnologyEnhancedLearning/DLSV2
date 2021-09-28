namespace DigitalLearningSolutions.Data.Enums
{
    // TODO HEEDLS-501 do I want to handle these differently? a more complex enumeration-class approach?
    // TODO HEEDLS-501 add documentation for this to swiki
    // these values are stored in the database and should not be altered or reorganised
    public enum RemovalMethod
    {
        Invalid, // this value should not be written to the database
        RemovedByDelegate,
        RemovedByAdmin,
        RemovedByGroupManagement,
        RemovedDuringRefresh
    }
}
