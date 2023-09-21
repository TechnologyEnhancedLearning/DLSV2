using DigitalLearningSolutions.Data.Models.User;
using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SuperAdmin
{
    public class SuperAdminDelegateAccount:DelegateUser
    {
        public SuperAdminDelegateAccount() { }
        public SuperAdminDelegateAccount(DelegateEntity delegateEntity) {
            Id = delegateEntity.DelegateAccount.Id;
            FirstName = delegateEntity.UserAccount.FirstName;
            LastName = delegateEntity.UserAccount.LastName;
            EmailAddress = delegateEntity.UserCentreDetails?.Email ?? delegateEntity.UserAccount.PrimaryEmail;
            UserId = delegateEntity.DelegateAccount.UserId;
            CentreName = delegateEntity.DelegateAccount.CentreName;
            CentreEmail = delegateEntity.UserCentreDetails?.Email;
            CandidateNumber = delegateEntity.DelegateAccount.CandidateNumber;
            LearningHubAuthId = delegateEntity.UserAccount.LearningHubAuthId;
            RegistrationConfirmationHash = delegateEntity.DelegateAccount.RegistrationConfirmationHash;
            DateRegistered = delegateEntity.DelegateAccount.DateRegistered;
            SelfReg = delegateEntity.DelegateAccount.SelfReg;
            Active = delegateEntity.DelegateAccount.Active;
            EmailVerified = delegateEntity.UserAccount.EmailVerified;
            UserActive = delegateEntity.UserAccount.Active;
            Approved = delegateEntity.DelegateAccount.Approved;

        }
        public bool SelfReg { get; set; }
        public string? CentreEmail { get; set; }
        public int? LearningHubAuthId { get; set; }
        public bool UserActive { get; set; }
    }
}
