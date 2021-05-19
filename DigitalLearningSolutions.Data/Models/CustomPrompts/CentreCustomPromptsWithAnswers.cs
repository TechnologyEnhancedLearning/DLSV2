namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CentreCustomPromptsWithAnswers
    {
        public CentreCustomPromptsWithAnswers(int centreId, List<CustomPromptWithAnswer> customPrompts)
        {
            CentreId = centreId;
            CustomPrompts = customPrompts;
        }

        public int CentreId { get; set; }

        public List<CustomPromptWithAnswer> CustomPrompts { get; set; }
    }
}
