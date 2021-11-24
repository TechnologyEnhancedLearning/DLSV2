namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CourseAdminFields
    {
        public CourseAdminFields(int customisationId, List<CustomPrompt> adminFields)
        {
            CustomisationId = customisationId;
            AdminFields = adminFields;
        }

        public int CustomisationId { get; set; }

        public List<CustomPrompt> AdminFields { get; set; }
    }
}
