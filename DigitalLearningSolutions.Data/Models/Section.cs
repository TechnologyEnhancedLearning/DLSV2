namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;

    public class Section
    {
        public Section()
        {
            Tutorials = new List<Tutorial>();
        }

        public Section(int sectionId, string sectionName)
        {
            SectionId = sectionId;
            SectionName = sectionName;
            Tutorials = new List<Tutorial>();
        }

        public Section(int sectionId, string sectionName, IEnumerable<Tutorial> tutorials)
        {
            SectionId = sectionId;
            SectionName = sectionName;
            Tutorials = tutorials;
        }

        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<Tutorial> Tutorials { get; set; }
    }
}
