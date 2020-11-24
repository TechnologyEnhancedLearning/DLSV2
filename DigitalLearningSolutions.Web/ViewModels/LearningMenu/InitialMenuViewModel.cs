namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class InitialMenuViewModel
    {
        public CourseContent CourseContent { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            CourseContent = courseContent;
        }
    }
}
