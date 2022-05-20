using DigitalLearningSolutions.Data.Models;

namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    public class BrandCourseTutorialViewModel
    {
        public BrandCourseTutorialViewModel(Tutorial t)
        {
            TutorialName = t.TutorialName;
        }

        public string TutorialName { get; set; }
    }
}
