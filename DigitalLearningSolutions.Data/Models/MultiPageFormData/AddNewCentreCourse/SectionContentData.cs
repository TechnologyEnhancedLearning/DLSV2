namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class SectionContentData
    {
        // 'Unused' constructor required for JsonConvert
        public SectionContentData(){}

        public SectionContentData(
            IEnumerable<Tutorial> tutorials
        )
        {
            Tutorials = tutorials.Select(t => new CourseTutorialData(t));
        }

        public SectionContentData(
            IEnumerable<CourseTutorialData> tutorials
        )
        {
            Tutorials = tutorials;
        }

        public IEnumerable<CourseTutorialData> Tutorials { get; set; }
    }
}
