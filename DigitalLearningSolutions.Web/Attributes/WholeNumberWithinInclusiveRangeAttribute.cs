namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;

    public class WholeNumberWithinInclusiveRangeAttribute : ValidationAttribute
    {
        private readonly string errorMessage;
        private readonly int lowerBound;
        private readonly int upperBound;

        public WholeNumberWithinInclusiveRangeAttribute(int lowerBound, int upperBound, string errorMessage)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string strValue when int.TryParse(strValue, out var intValue) && intValue >= lowerBound &&
                                          intValue <= upperBound:
                    return ValidationResult.Success;
                default:
                    return new ValidationResult(errorMessage);
            }
        }
    }
}
