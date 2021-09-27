﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash);

        IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailAndCentre(int centreId, string email);

        void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateId);

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

        public SupervisorDelegate? GetSupervisorDelegateRecordByInviteHash(Guid inviteHash)
        {
            return supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash);
        }

        public IEnumerable<SupervisorDelegate> GetPendingSupervisorDelegateRecordsByEmailAndCentre(int centreId, string email)
        {
            return supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(centreId, email);
        }

        public void AddDelegateIdToSupervisorDelegateRecords(IEnumerable<int> supervisorDelegateIds, int delegateId)
        {
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(supervisorDelegateIds, delegateId);
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
