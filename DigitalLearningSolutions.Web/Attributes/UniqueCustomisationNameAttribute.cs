namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Services;

    public class UniqueCustomisationNameAttribute : ValidationAttribute
    {
        private readonly int applicationId;
        private readonly int centreId;
        private readonly ICourseService courseService;
        private readonly string errorMessage;

        public UniqueCustomisationNameAttribute(
            ICourseService courseService,
            int centreId,
            int applicationId,
            string errorMessage
        )
        {
            this.courseService = courseService;
            this.centreId = centreId;
            this.applicationId = applicationId;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string strValue
                    when courseService.DoesCourseNameExistAtCentre(strValue, centreId, applicationId):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult(errorMessage);
            }
        }
    }
}
