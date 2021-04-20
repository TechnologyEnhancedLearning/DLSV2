namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class LearnerInformationViewModel
    {
        public LearnerInformationViewModel()
        {
            Centre = 0;
            JobGroup = 0;
        }

        [Required(ErrorMessage = "Please select a centre to register.")]
        public int? Centre { get; set; }

        [Required(ErrorMessage = "Please select a job group to register.")]
        public int? JobGroup { get; set; }
    }
}
