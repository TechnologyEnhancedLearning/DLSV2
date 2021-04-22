namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class LearnerInformationViewModel
    {
        [Required(ErrorMessage = "Please select a centre.")]
        public int? Centre { get; set; }

        [Required(ErrorMessage = "Please select a job group.")]
        public int? JobGroup { get; set; }
    }
}
