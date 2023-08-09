namespace DigitalLearningSolutions.Web.ViewModels.CentreCourses
{
    using System.Collections.Generic;

    public class CentreCoursesViewModel
    {
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public List<CentreCourseCustomisation> CentreCourseCustomisations { get; set; }
    }

    public class CentreCourseCustomisation
    {
        public int CustomisationID { get; set; }
        public string CustomisationName { get; set; }
        public int DelegateCount { get; set;}
    }
}
