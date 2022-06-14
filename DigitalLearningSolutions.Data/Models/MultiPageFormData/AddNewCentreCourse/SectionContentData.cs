namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class SectionContentData
    {
        // 'Unused' constructor required for JsonConvert
        public SectionContentData(){}

        public SectionContentData(
            Section section,
            int index,
            bool showDiagnostic,
            IEnumerable<Tutorial> tutorials
        )
        {
            SectionName = section.SectionName;
            Index = index;
            ShowDiagnostic = showDiagnostic;
            Tutorials = tutorials.Select(t => new CourseTutorialData(t));
        }

        public SectionContentData(
            string sectionName,
            bool showDiagnostic,
            IEnumerable<CourseTutorialData> tutorials,
            int index
        )
        {
            SectionName = sectionName;
            ShowDiagnostic = showDiagnostic;
            Tutorials = tutorials;
            Index = index;
        }

        public int Index { get; set; }
        public string SectionName { get; set; }
        public bool ShowDiagnostic { get; set; }
        public IEnumerable<CourseTutorialData> Tutorials { get; set; }
    }
}
