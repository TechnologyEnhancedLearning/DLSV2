namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;

    public class ChooseACentreAccount
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

        private const string ChooseCentreButtonLabel = "Choose";
        private const string ReactivateAccountButtonLabel = "Reactivate";

        public ChooseACentreAccount(
            int centreId,
            string centreName,
            bool isCentreActive = false,
            bool isActiveAdmin = false,
            bool isDelegate = false,
            bool isDelegateApproved = false,
            bool isDelegateActive = false
        )
        {
            CentreId = centreId;
            CentreName = centreName;
            IsCentreInactive = !isCentreActive;
            IsActiveAdmin = isActiveAdmin;
            IsDelegate = isDelegate;
            IsDelegateApproved = isDelegateApproved;
            IsDelegateInactive = IsDelegate && !isDelegateActive;
        }

        public int CentreId { get; }
        public string CentreName { get; }
        public bool IsActiveAdmin { get; }
        public bool IsDelegate { get; }
        private bool IsCentreInactive { get; }
        private bool IsDelegateApproved { get; }
        private bool IsDelegateInactive { get; }
        private bool IsDelegateActive => IsDelegate && !IsDelegateInactive;
        public bool IsDelegateUnapproved => IsDelegate && !IsDelegateApproved;

        public Dictionary<string, string> StatusTag
        {
            get
            {
                var label = ActiveLabel;

                if (IsCentreInactive)
                {
                    label = CentreInactiveLabel;
                }
                else if (IsDelegateInactive)
                {
                    label = IsActiveAdmin ? DelegateInactiveLabel : InactiveLabel;
                }
                else if (IsDelegateUnapproved)
                {
                    label = IsActiveAdmin ? DelegateUnapprovedLabel : UnapprovedLabel;
                }

                var colour = label == ActiveLabel
                    ? ActiveColor
                    : label == DelegateInactiveLabel || label == InactiveLabel
                        ? InactiveColor
                        : UnapprovedOrCentreInactiveColour;

                return new Dictionary<string, string>
                {
                    { "Label", label },
                    { "Colour", colour },
                };
            }
        }

        public string? ActionButton
        {
            get
            {
                if (IsCentreInactive)
                {
                    return null;
                }

                return IsActiveAdmin || (IsDelegateApproved && IsDelegateActive)
                    ? ChooseCentreButtonLabel
                    : IsDelegateInactive
                        ? ReactivateAccountButtonLabel
                        : null;
            }
        }
    }
}
