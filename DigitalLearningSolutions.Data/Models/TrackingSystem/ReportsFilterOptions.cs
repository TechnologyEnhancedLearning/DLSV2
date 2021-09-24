namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System.Collections.Generic;

    public class ReportsFilterOptions
    {
        public ReportsFilterOptions(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<(int id, string name)> categories,
            IEnumerable<(int id, string name)> courses
        )
        {
            JobGroups = jobGroups;
            Categories = categories;
            Courses = courses;
        }

        public IEnumerable<(int id, string name)> JobGroups { get; set; }
        public IEnumerable<(int id, string name)> Categories { get; set; }
        public IEnumerable<(int id, string name)> Courses { get; set; }
    }
}
