namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    public interface ILearningResourceCardViewModel
    {
        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
