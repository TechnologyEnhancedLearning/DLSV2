namespace DigitalLearningSolutions.Web.ViewModels.RoleProfiles
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using System.Collections.Generic;

    public class ProfessionalGroupViewModel
    {
        public IEnumerable<NRPProfessionalGroups> NRPProfessionalGroups { get; set; }
        public RoleProfileBase RoleProfileBase { get; set; }
    }
}
