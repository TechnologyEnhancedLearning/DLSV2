namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class InitialMenuViewModel
    {
        public CourseContent CourseContent { get; }
        public IEnumerable<SectionCardViewModel> Sections { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            CourseContent = courseContent;
            Sections = courseContent.Sections.Select(section => new SectionCardViewModel(section));
        }
    }
}
