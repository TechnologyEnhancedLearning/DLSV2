namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AddRegistrationPromptSelectPromptViewModel
    {
        public AddRegistrationPromptSelectPromptViewModel()
        { }

        public AddRegistrationPromptSelectPromptViewModel(int customPromptId, bool mandatory)
        {
            CustomPromptId = customPromptId;
            Mandatory = mandatory;
        }

        [Required(ErrorMessage = "Select a prompt name")]
        public int? CustomPromptId { get; set; }

        public bool Mandatory { get; set; }

        public bool CustomPromptIdIsInPromptIdList(IEnumerable<int> idList)
        {
            return CustomPromptId.HasValue && idList.Contains(CustomPromptId.Value);
        }
    }
}
