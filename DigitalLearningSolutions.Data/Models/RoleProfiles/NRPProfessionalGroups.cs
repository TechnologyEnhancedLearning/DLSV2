namespace DigitalLearningSolutions.Data.Models.RoleProfiles
{
    using System.ComponentModel.DataAnnotations;
    public class NRPProfessionalGroups
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string ProfessionalGroup { get; set; }
        public bool Active { get; set; }
    }
}
