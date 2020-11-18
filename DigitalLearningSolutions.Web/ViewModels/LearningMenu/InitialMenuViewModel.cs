namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class InitialMenuViewModel
    {
        public string Name { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Name = $"{courseContent.ApplicationName} - {courseContent.CustomisationName}";
        }
    }
}
