using FluentMigrator.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DeactivateDelegate
{
    public class DeactivateDelegateAccountViewModel
    {
        public int DelegateId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        [Required(ErrorMessage = "Please select an account you want to deactivate.")]
        public bool? Deactivate { get; set; }
    }
}
