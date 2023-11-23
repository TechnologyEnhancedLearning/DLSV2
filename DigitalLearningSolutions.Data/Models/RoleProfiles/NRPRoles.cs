namespace DigitalLearningSolutions.Data.Models.RoleProfiles
{
    using System.ComponentModel.DataAnnotations;
    public class NRPRoles
    {
        public int ID { get; set; }
        public int NRPSubGroupID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string RoleProfile { get; set; }
        public bool Active { get; set; }
    }
}
