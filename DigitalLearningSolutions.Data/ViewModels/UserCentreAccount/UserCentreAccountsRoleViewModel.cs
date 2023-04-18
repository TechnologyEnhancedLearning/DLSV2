using DigitalLearningSolutions.Data.Enums;

namespace DigitalLearningSolutions.Data.ViewModels.UserCentreAccount
{
    public class UserCentreAccountsRoleViewModel
    {
        public readonly int CentreId;
        public readonly string CentreName;
        public readonly bool IsActiveAdmin;
        private readonly bool isApprovedDelegate;
        private readonly bool isCentreInactive;
        public readonly bool IsDelegate;
        private readonly bool isInactiveDelegate;
        private readonly bool isUnverifiedEmail;

        public UserCentreAccountsRoleViewModel(
            int centreId,
            string centreName,
            bool isCentreActive,
            bool isActiveAdmin,
            bool isDelegate,
            bool isDelegateApproved,
            bool isDelegateActive,
            bool isEmailUnverified
        )
        {
            CentreId = centreId;
            CentreName = centreName;
            isCentreInactive = !isCentreActive;
            IsActiveAdmin = isActiveAdmin;
            IsDelegate = isDelegate;
            isApprovedDelegate = IsDelegate && isDelegateApproved;
            isInactiveDelegate = IsDelegate && !isDelegateActive;
            isUnverifiedEmail = isEmailUnverified;
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
                if (isUnverifiedEmail)
                {
                    return ChooseACentreStatus.EmailUnverified;
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
