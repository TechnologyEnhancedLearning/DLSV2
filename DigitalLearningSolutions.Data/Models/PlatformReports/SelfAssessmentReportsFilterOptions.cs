namespace DigitalLearningSolutions.Data.Models.PlatformReports
{
    using System.Collections.Generic;

    public class SelfAssessmentReportsFilterOptions
    {
        public SelfAssessmentReportsFilterOptions(
            IEnumerable<(int id, string name)> centreTypes,
            IEnumerable<(int id, string name)> regions,
            IEnumerable<(int id, string name)> centres,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<(int id, string name)> brands,
            IEnumerable<(int id, string name)> categories,
            IEnumerable<(int id, string name)> selfAssessments
        )
        {
            CentreTypes = centreTypes;
            Regions = regions;
            Centres = centres;
            JobGroups = jobGroups;
            Categories = categories;
            Brands = brands;
            SelfAssessments = selfAssessments;
        }
        public IEnumerable<(int id, string name)> CentreTypes { get; set; }
        public IEnumerable<(int id, string name)> Regions { get; set; }
        public IEnumerable<(int id, string name)> Centres { get; set; }
        public IEnumerable<(int id, string name)> JobGroups { get; set; }
        public IEnumerable<(int id, string name)> Categories { get; set; }
        public IEnumerable<(int id, string name)> Brands { get; set; }
        public IEnumerable<(int id, string name)> SelfAssessments { get; set; }
    }
}
