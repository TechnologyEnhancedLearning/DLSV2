namespace DigitalLearningSolutions.Data.Models
{
    public class CentreApplication
    {
        public CentreApplication(
           int centreApplicationId,
           int centreId,
           string? centreName,
           int applicationId,
           string? applicationName,
           int customisationCount
       )
        {
            CentreApplicationID = centreApplicationId;
            ApplicationID = applicationId;
            CentreID = centreId;
            CentreName = centreName;
            ApplicationName = applicationName;
            CustomisationCount = customisationCount;
        }
        public int CentreApplicationID { get; set; }
        public int CentreID { get; set; }
        public string? CentreName { get; set; }
        public int ApplicationID { get; set; }
        public string? ApplicationName { get; set; }
        public int CustomisationCount { get; set; }
    }
}
