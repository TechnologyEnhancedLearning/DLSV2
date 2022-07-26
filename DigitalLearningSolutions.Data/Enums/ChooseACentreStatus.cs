namespace DigitalLearningSolutions.Data.Enums
{
    public class ChooseACentreStatus : Enumeration
    {
        // These colours form part of a CSS class: nhsuk-tag--[tagColour]
        private const string GreenCssClassName = "green";
        private const string RedCssClassName = "red";
        private const string GreyCssClassName = "grey";

        public static readonly ChooseACentreStatus Active = new ChooseACentreStatus(
            0,
            nameof(Active),
            "Active",
            GreenCssClassName,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus Inactive = new ChooseACentreStatus(
            1,
            nameof(Inactive),
            "Inactive",
            RedCssClassName,
            ChooseACentreButton.Reactivate
        );

        public static readonly ChooseACentreStatus DelegateInactive = new ChooseACentreStatus(
            2,
            nameof(DelegateInactive),
            "Delegate inactive",
            RedCssClassName,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus Unapproved = new ChooseACentreStatus(
            3,
            nameof(Unapproved),
            "Unapproved",
            GreyCssClassName,
            null
        );

        public static readonly ChooseACentreStatus DelegateUnapproved = new ChooseACentreStatus(
            4,
            nameof(DelegateUnapproved),
            "Delegate unapproved",
            GreyCssClassName,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus CentreInactive = new ChooseACentreStatus(
            5,
            nameof(CentreInactive),
            "Centre inactive",
            GreyCssClassName,
            null
        );

        public static readonly ChooseACentreStatus EmailUnverified = new ChooseACentreStatus(
            5,
            nameof(EmailUnverified),
            "Email unverified",
            RedCssClassName,
            null
        );

        public readonly ChooseACentreButton? ActionButton;
        public readonly string TagColour;

        public readonly string TagLabel;

        private ChooseACentreStatus(
            int id,
            string name,
            string tagLabel,
            string tagColour,
            ChooseACentreButton? actionButton
        ) : base(id, name)
        {
            TagLabel = tagLabel;
            TagColour = tagColour;
            ActionButton = actionButton;
        }
    }
}
