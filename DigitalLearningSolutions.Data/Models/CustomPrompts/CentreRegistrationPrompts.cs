namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CentreRegistrationPrompts
    {
        public CentreRegistrationPrompts(int centreId, List<CentreRegistrationPrompt> customPrompts)
        {
            CentreId = centreId;
            CustomPrompts = customPrompts;
        }

        public int CentreId { get; set; }

        public List<CentreRegistrationPrompt> CustomPrompts { get; set; }
    }
}
