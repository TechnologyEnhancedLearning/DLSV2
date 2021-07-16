namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CourseCustomPrompts
    {
        public CourseCustomPrompts(int customisationId, int centreId, List<CustomPrompt> courseAdminFields)
        {
            CustomisationId = customisationId;
            CentreId = centreId;
            CourseAdminFields = courseAdminFields;
        }

        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public List<CustomPrompt> CourseAdminFields { get; set; }
    }
}
