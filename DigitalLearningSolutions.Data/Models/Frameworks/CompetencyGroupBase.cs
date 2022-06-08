namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.ComponentModel.DataAnnotations;
    public class CompetencyGroupBase
    {
        public int ID { get; set; }
        public int CompetencyGroupID { get; set; }

        [StringLength(maximumLength: 255, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
