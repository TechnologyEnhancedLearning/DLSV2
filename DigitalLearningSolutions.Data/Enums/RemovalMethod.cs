namespace DigitalLearningSolutions.Data.Enums
{
    // these values are stored in the database and should not be altered or reordered
    public enum RemovalMethod
    {
        NotRemoved,
        RemovedByDelegate,
        RemovedByAdmin,
        RemovedByGroupManagement,
        RemovedDuringRefresh
    }
}
