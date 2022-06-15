namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

    public class ChooseACentreAccountViewModel
    {
        public readonly int CentreId;
        public readonly string CentreName;
        public readonly bool IsActiveAdmin;
        public readonly bool IsDelegate;
        private readonly bool isCentreInactive;
        private readonly bool isApprovedDelegate;
        private readonly bool isInactiveDelegate;

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
            isCentreInactive = !isCentreActive;
            IsActiveAdmin = isActiveAdmin;
            IsDelegate = isDelegate;
            isApprovedDelegate = IsDelegate && isDelegateApproved;
            isInactiveDelegate = IsDelegate && !isDelegateActive;
        }

        public bool IsUnapprovedDelegate => IsDelegate && !isApprovedDelegate;

        public ChooseACentreStatus Status
        {
            get
            {
                if (isCentreInactive)
                {
                    return ChooseACentreStatus.CentreInactive;
                }

                if (isInactiveDelegate)
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
