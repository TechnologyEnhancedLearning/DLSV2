namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class NRPSubGroups
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string SubGroup { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
