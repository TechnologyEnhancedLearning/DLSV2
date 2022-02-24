namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CourseAdminFields
    {
        public CourseAdminFields(int customisationId, List<CoursePrompt> adminFields)
        {
            CustomisationId = customisationId;
            AdminFields = adminFields;
        }

        public int CustomisationId { get; set; }

        public List<CoursePrompt> AdminFields { get; set; }
    }
}
