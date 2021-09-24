namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(int centreId, Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailAndCentre(int centreId, string email);

        void AddCandidateIdToSupervisorDelegateRecords(
            IEnumerable<int> supervisorDelegateIds,
            int centreId,
            string candidateNumber
        );

        void ConfirmSupervisorDelegateRecord(int supervisorDelegateId);
    }

    public class SupervisorDelegateService : ISupervisorDelegateService
    {
        private readonly ISupervisorDelegateDataService supervisorDelegateDataService;
        private readonly IUserDataService userDataService;

        public SupervisorDelegateService(
            ISupervisorDelegateDataService supervisorDelegateDataService,
            IUserDataService userDataService
        )
        {
            this.supervisorDelegateDataService = supervisorDelegateDataService;
            this.userDataService = userDataService;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(int centreId, Guid inviteHash)
        {
            var record = supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash);
            return record?.CentreId == centreId ? record : null;
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailAndCentre(int centreId, string email)
        {
            return supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(centreId, email);
        }

        public void AddCandidateIdToSupervisorDelegateRecords(
            IEnumerable<int> supervisorDelegateIds,
            int centreId,
            string candidateNumber
        )
        {
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(candidateNumber, centreId)!;
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(
                supervisorDelegateIds,
                delegateUser.Id
            );
        }

        public void ConfirmSupervisorDelegateRecord(int supervisorDelegateId)
        {
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordConfirmed(
                supervisorDelegateId,
                DateTime.UtcNow
            );
        }
    }
}
