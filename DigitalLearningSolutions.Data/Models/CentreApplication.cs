using System;

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
       )
        {
            CentreApplicationID = centreApplicationId;
            ApplicationID = applicationId;
            CentreID = centreId;
            ApplicationName = applicationName;
        }
        public int CentreApplicationID { get; set; }
        public int CentreID { get; set; }
        public string? CentreName { get; set; }
        public int ApplicationID { get; set; }
        public string? ApplicationName { get; set; }
        
    }
}
