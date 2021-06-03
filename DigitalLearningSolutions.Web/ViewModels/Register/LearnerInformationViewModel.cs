namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class LearnerInformationViewModel
    {
        [Required(ErrorMessage = "Select a job group")]
        public int? JobGroup { get; set; }
    }
}
