namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;

    public class ChooseACentreAccountViewModel
    {
        public readonly int CentreId;
        public readonly string CentreName;
        public readonly bool IsActiveAdmin;
        private readonly bool isApprovedDelegate;
        private readonly bool isCentreInactive;
        public readonly bool IsDelegate;
        private readonly bool isInactiveDelegate;
        private readonly bool isUnverifiedEmail;

        public ChooseACentreAccountViewModel(
            int centreId,
            string centreName,
            bool isCentreActive,
            bool isActiveAdmin,
            bool isDelegate,
            bool isDelegateApproved,
            bool isDelegateActive,
            ICollection<int> idsOfCentresWithUnverifiedEmails
        )
        {
            CentreId = centreId;
            CentreName = centreName;
            isCentreInactive = !isCentreActive;
            IsActiveAdmin = isActiveAdmin;
            IsDelegate = isDelegate;
            isApprovedDelegate = IsDelegate && isDelegateApproved;
            isInactiveDelegate = IsDelegate && !isDelegateActive;
            isUnverifiedEmail = idsOfCentresWithUnverifiedEmails.Contains(CentreId);
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
