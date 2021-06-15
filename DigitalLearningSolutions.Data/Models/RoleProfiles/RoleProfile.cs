namespace DigitalLearningSolutions.Data.Models.RoleProfiles
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class RoleProfile : RoleProfileBase
    {
        public DateTime CreatedDate { get; set; }
        public string? Brand
        {
            get => brand;
            set => brand = GetValidOrNull(value);
        }
        public string? ParentRoleProfile { get; set; }
        public string? Owner { get; set; }
        public DateTime? Archived { get; set; }
        public DateTime LastEdit { get; set; }
        public string? NRPProfessionalGroup { get; set; }
        public string? NRPSubGroup { get; set; }
        public string? NRPRole { get; set; }
        public int? RoleProfileReviewID { get; set; }
        private string? brand;
        private static string? GetValidOrNull(string? toValidate)
        {
            return toValidate != null && toValidate.ToLower() == "undefined" ? null : toValidate;
        }
    }
}
