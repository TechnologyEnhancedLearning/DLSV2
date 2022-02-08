namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CustomPromptWithResponseCounts : CustomPrompt
    {
        public CustomPromptWithResponseCounts(int customPromptNumber, string text, string? options, bool mandatory) :
            base(customPromptNumber, text, options, mandatory) { }

        public IEnumerable<ResponseCount> ResponseCounts { get; set; }
    }
}
