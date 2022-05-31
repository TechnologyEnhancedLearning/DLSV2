namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;

    public class ChooseACentreAccount
    {
        public ChooseACentreAccount(
            int centreId,
            string centreName,
            bool isCentreActive = false,
            bool isAdmin = false,
            bool isDelegate = false,
            bool isDelegateApproved = false,
            bool isDelegateActive = false
        )
        {
            CentreId = centreId;
            CentreName = centreName;
            IsCentreInactive = !isCentreActive;
            IsAdmin = isAdmin;
            IsDelegate = isDelegate;
            IsDelegateApproved = isDelegateApproved;
            IsDelegateInactive = IsDelegate && !isDelegateActive;
        }

        public int CentreId { get; }
        public string CentreName { get; }
        public bool IsAdmin { get; }
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
                string label = "Active";

                if (IsCentreInactive)
                {
                    label = "Centre inactive";
                }
                else if (IsDelegateInactive)
                {
                    label = IsAdmin ? "Delegate inactive" : "Inactive";
                }
                else if (IsDelegateUnapproved)
                {
                    label = IsAdmin ? "Delegate unapproved" : "Unapproved";
                }

                var colour = label == "Active"
                    ? "green"
                    : label == "Delegate inactive" || label == "Inactive"
                        ? "red"
                        : "grey";

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

                return IsAdmin || (IsDelegateApproved && IsDelegateActive)
                    ? "Choose"
                    : IsDelegateInactive
                        ? "Reactivate"
                        : null;
            }
        }
    }
}
