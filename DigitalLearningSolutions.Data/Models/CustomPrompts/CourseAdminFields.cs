namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CourseAdminFields
    {
        public CourseAdminFields(int customisationId, int centreId, List<CustomPrompt> adminFields)
        {
            CustomisationId = customisationId;
            CentreId = centreId;
            AdminFields = adminFields;
        }

        public int CustomisationId { get; set; }
        public int CentreId { get; set; }

        public List<CustomPrompt> AdminFields { get; set; }
    }
}
