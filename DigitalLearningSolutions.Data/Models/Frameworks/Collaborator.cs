namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class Collaborator
    {
        public int FrameworkID { get; set; }
        public int AdminID { get; set; }
        public bool CanModify { get; set; }
    }
}
