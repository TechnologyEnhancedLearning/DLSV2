namespace DigitalLearningSolutions.Data.Enums
{
    public class ButtonType : Enumeration
    {
        public static readonly ButtonType Primary = new ButtonType(
            0,
            nameof(Primary),
            "nhsuk-button"
        );

        public static readonly ButtonType Secondary = new ButtonType(
            1,
            nameof(Secondary),
            "nhsuk-button nhsuk-button--secondary"
        );

        public static readonly ButtonType Reverse = new ButtonType(
            2,
            nameof(Reverse),
            "nhsuk-button nhsuk-button--reverse"
        );

        public readonly string CssClass;

        private ButtonType(
            int id,
            string name,
            string cssClass
        ) : base(id, name)
        {
            CssClass = cssClass;
        }
    }
}
