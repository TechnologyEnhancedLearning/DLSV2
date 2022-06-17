namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class SectionContentTempData
    {
        // 'Unused' constructor required for JsonConvert
        public SectionContentTempData(){}

        public SectionContentTempData(
            IEnumerable<Tutorial> tutorials
        )
        {
            Tutorials = tutorials.Select(t => new CourseTutorialTempData(t));
        }

        public SectionContentTempData(
            IEnumerable<CourseTutorialTempData> tutorials
        )
        {
            Tutorials = tutorials;
        }

        public IEnumerable<CourseTutorialTempData> Tutorials { get; set; }
    }
}
