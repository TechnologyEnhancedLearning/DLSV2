namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System;
    using System.Collections.Generic;

    public class CourseAdminField : Prompt
    {
        public CourseAdminField(int promptNumber, string promptText, string? options) : base(promptText, options)
        {
            PromptNumber = promptNumber;
        }

        public int PromptNumber { get; set; }
    }
}
