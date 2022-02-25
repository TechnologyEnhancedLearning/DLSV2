namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System;
    using System.Collections.Generic;

    public class CourseAdminField : Prompt
    {
        public CourseAdminField(int promptNumber, string text, string? options)
        {
            PromptNumber = promptNumber;
            PromptText = text;
            Options = SplitOptionsString(options);
        }

        public int PromptNumber { get; set; }
    }
}
