namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class NRPRoles
    {
        public int ID { get; set; }
        public int NRPSubGroupID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string ProfileName { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
