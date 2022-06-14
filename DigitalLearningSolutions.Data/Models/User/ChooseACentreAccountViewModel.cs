namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

    public class ChooseACentreAccountViewModel
    {
        public ChooseACentreAccountViewModel(
            int centreId,
            string centreName,
            bool isCentreActive,
            bool isActiveAdmin,
            bool isDelegate,
            bool isDelegateApproved,
            bool isDelegateActive
        )
        {
            CentreId = centreId;
            CentreName = centreName;
            IsCentreInactive = !isCentreActive;
            IsActiveAdmin = isActiveAdmin;
            IsDelegate = isDelegate;
            IsApprovedDelegate = IsDelegate && isDelegateApproved;
            IsInactiveDelegate = IsDelegate && !isDelegateActive;
        }

        public int CentreId { get; }
        public string CentreName { get; }
        public bool IsActiveAdmin { get; }
        public bool IsDelegate { get; }
        private bool IsCentreInactive { get; }
        private bool IsApprovedDelegate { get; }
        private bool IsInactiveDelegate { get; }
        public bool IsUnapprovedDelegate => IsDelegate && !IsApprovedDelegate;

        public ChooseACentreStatus Status
        {
            get
            {
                if (IsCentreInactive)
                {
                    return ChooseACentreStatus.CentreInactive;
                }

                if (IsInactiveDelegate)
                {
                    return IsActiveAdmin ? ChooseACentreStatus.DelegateInactive : ChooseACentreStatus.Inactive;
                }

                if (IsUnapprovedDelegate)
                {
                    return IsActiveAdmin ? ChooseACentreStatus.DelegateUnapproved : ChooseACentreStatus.Unapproved;
                }

                return ChooseACentreStatus.Active;
            }
        }
    }
}
