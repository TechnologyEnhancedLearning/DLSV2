namespace DigitalLearningSolutions.Data.Enums
{
    public class ChooseACentreStatus : Enumeration
    {
        private const string ActiveLabel = "Active";
        private const string InactiveLabel = "Inactive";
        private const string UnapprovedLabel = "Unapproved";
        private const string DelegateInactiveLabel = "Delegate inactive";
        private const string DelegateUnapprovedLabel = "Delegate unapproved";
        private const string CentreInactiveLabel = "Centre inactive";

        private const string ActiveColor = "green";
        private const string InactiveColor = "red";
        private const string UnapprovedOrCentreInactiveColour = "grey";

        public static readonly ChooseACentreStatus Active = new ChooseACentreStatus(
            0,
            nameof(Active),
            ActiveLabel,
            ActiveColor,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus Inactive = new ChooseACentreStatus(
            1,
            nameof(Inactive),
            InactiveLabel,
            InactiveColor,
            ChooseACentreButton.Reactivate
        );

        public static readonly ChooseACentreStatus DelegateInactive = new ChooseACentreStatus(
            2,
            nameof(DelegateInactive),
            DelegateInactiveLabel,
            InactiveColor,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus Unapproved = new ChooseACentreStatus(
            3,
            nameof(Unapproved),
            UnapprovedLabel,
            UnapprovedOrCentreInactiveColour,
            null
        );

        public static readonly ChooseACentreStatus DelegateUnapproved = new ChooseACentreStatus(
            4,
            nameof(DelegateUnapproved),
            DelegateUnapprovedLabel,
            UnapprovedOrCentreInactiveColour,
            ChooseACentreButton.Choose
        );

        public static readonly ChooseACentreStatus CentreInactive = new ChooseACentreStatus(
            5,
            nameof(CentreInactive),
            CentreInactiveLabel,
            UnapprovedOrCentreInactiveColour,
            null
        );

        public readonly string TagLabel;
        public readonly string TagColour;
        public readonly ChooseACentreButton? ActionButton;

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
