namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt
{
    using System.Collections.Generic;
    using System.Linq;

    public class AddRegistrationPromptSelectPromptData
    {
        public AddRegistrationPromptSelectPromptData()
        { }

        public AddRegistrationPromptSelectPromptData(int? customPromptId, bool mandatory)
        {
            CustomPromptId = customPromptId;
            Mandatory = mandatory;
        }

        public int? CustomPromptId { get; set; }

        public bool Mandatory { get; set; }

        public bool CustomPromptIdIsInPromptIdList(IEnumerable<int> idList)
        {
            return CustomPromptId.HasValue && idList.Contains(CustomPromptId.Value);
        }
    }
}
