namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string?> emails
        );

        void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateUserId);
    }

    public class SupervisorDelegateService : ISupervisorDelegateService
    {
        private readonly ISupervisorDelegateDataService supervisorDelegateDataService;

        public SupervisorDelegateService(
            ISupervisorDelegateDataService supervisorDelegateDataService
        )
        {
            this.supervisorDelegateDataService = supervisorDelegateDataService;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash)
        {
            return supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash);
        }

        // TODO: HEEDLS-1014 - Change name of method to AddUserIdToSupervisorDelegateRecords
        public void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateUserId)
        {
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(supervisorDelegateIds, delegateUserId);
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string?> emails
        )
        {
            var nonNullEmails = emails.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (!nonNullEmails.Any())
            {
                return new List<SupervisorDelegate>();
            }

            return supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                centreId,
                nonNullEmails!
            );
        }
    }
}
