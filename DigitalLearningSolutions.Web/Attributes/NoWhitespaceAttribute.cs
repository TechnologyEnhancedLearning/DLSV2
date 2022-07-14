namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    ///     Specifies that no whitespace characters are allowed
    /// </summary>
    public class NoWhitespaceAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            switch (value)
            {
                case null:
                case string strValue when !strValue.Any(char.IsWhiteSpace):
                    return true;
                default:
                    return false;
            }
        }
    }
}
