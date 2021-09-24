namespace DigitalLearningSolutions.Data.Enums
{
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
