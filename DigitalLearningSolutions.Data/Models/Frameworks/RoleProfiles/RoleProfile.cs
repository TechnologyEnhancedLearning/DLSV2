namespace DigitalLearningSolutions.Data.Models.RoleProfiles
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class RoleProfile
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string RoleProfileName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BrandID { get; set; }
        public string? Brand
        {
            get => brand;
            set => brand = GetValidOrNull(value);
        }
        public int ParentRoleProfileID { get; set; }
        public string? ParentRoleProfile { get; set; }
        public bool National { get; set; }
        public bool Public { get; set; }
        public int OwnerAdminID { get; set; }
        public string? Owner { get; set; }
        public DateTime? Archived { get; set; }
        public DateTime LastEdit { get; set; }
        public int? NRPProfessionalGroupID { get; set; }
        public string? NRPProfessionalGroup { get; set; }
        public int? NRPSubGroupID { get; set; }
        public string? NRPSubGroup { get; set; }
        public int? NRPRoleID { get; set; }
        public string? NRPRole { get; set; }
        public int PublishStatusID { get; set; }
        public string? PublishStatus { get; set; }
        public int UserRole { get; set; }
        public int? RoleProfileReviewID { get; set; }
        private string? brand;
        private static string? GetValidOrNull(string? toValidate)
        {
            return toValidate != null && toValidate.ToLower() == "undefined" ? null : toValidate;
        }
    }
}
