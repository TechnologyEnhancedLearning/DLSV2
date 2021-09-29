namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    public class RemoveFromCourseViewModel
    {
        public int DelegateId { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Name => Forename + " " + Surname;
        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
    }
}
