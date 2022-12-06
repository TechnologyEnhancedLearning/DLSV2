namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using System.ComponentModel.DataAnnotations;
    public class FrameworkCompetency : BaseSearchableItem
    {
        public int Id { get; set; }
        public int CompetencyID { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Ordering { get; set; }
        public int AssessmentQuestions { get; set; }
        public int CompetencyLearningResourcesCount { get; set; }
        public string? FrameworkName { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? Name;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
