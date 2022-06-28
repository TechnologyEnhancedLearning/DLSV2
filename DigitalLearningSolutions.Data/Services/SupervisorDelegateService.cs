namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string?> emails
        );

        void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateId);
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

        public void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateId)
        {
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(supervisorDelegateIds, delegateId);
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
            int centreId,
            IEnumerable<string?> emails
        )
        {
            return supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                centreId,
                emails
            );
        }
    }
}
