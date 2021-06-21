namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class LearnerInformationViewModel
    {
        [Required(ErrorMessage = "Select a job group")]
        public int? JobGroup { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public List<EditCustomFieldViewModel> CustomFields { get; set; } = new List<EditCustomFieldViewModel>();

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
    }
}
