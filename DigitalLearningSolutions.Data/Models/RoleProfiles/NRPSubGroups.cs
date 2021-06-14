namespace DigitalLearningSolutions.Data.Models.RoleProfiles
{
    using System.ComponentModel.DataAnnotations;
    public class NRPSubGroups
    {
        public int ID { get; set; }
        public int NRPProfessionalGroupID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string SubGroup { get; set; }
        public bool Active { get; set; }
    }
}
