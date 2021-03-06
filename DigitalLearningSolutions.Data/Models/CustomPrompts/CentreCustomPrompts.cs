﻿namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CentreCustomPrompts
    {
        public CentreCustomPrompts(int centreId, List<CustomPrompt> customPrompts)
        {
            CentreId = centreId;
            CustomPrompts = customPrompts;
        }

        public int CentreId { get; set; }

        public List<CustomPrompt> CustomPrompts { get; set; }
    }
}
