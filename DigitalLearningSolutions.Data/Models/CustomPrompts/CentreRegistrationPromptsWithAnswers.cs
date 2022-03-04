namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CentreRegistrationPromptsWithAnswers
    {
        public CentreRegistrationPromptsWithAnswers(int centreId, List<CentreRegistrationPromptWithAnswer> customPrompts)
        {
            CentreId = centreId;
            CustomPrompts = customPrompts;
        }

        public int CentreId { get; set; }

        public List<CentreRegistrationPromptWithAnswer> CustomPrompts { get; set; }
    }
}
