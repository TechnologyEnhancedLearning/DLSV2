namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int centreId, int supervisorDelegateId);

        SupervisorDelegate? GetPendingSupervisorDelegateRecordByIdAndEmail(
            int centreId,
            int supervisorDelegateId,
            string email
        );

        SupervisorDelegate? GetPendingSupervisorDelegateRecordByEmail(int centreId, string email);

        void UpdateSupervisorDelegateRecordStatus(int supervisorDelegateId, int centreId, string candidateNumber, bool setConfirmed);
    }

    public class SupervisorDelegateService : ISupervisorDelegateService
    {
        private readonly ISupervisorDelegateDataService supervisorDelegateDataService;
        private readonly IUserDataService userDataService;

        public SupervisorDelegateService(ISupervisorDelegateDataService supervisorDelegateDataService, IUserDataService userDataService)
        {
            this.supervisorDelegateDataService = supervisorDelegateDataService;
            this.userDataService = userDataService;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecord(int centreId, int supervisorDelegateId)
        {
            var record = supervisorDelegateDataService.GetSupervisorDelegateRecord(supervisorDelegateId);
            return record?.CentreId == centreId ? record : null;
        }

        public SupervisorDelegate? GetPendingSupervisorDelegateRecordByIdAndEmail(
            int centreId,
            int supervisorDelegateId,
            string email
        )
        {
            var record = GetSupervisorDelegateRecord(centreId, supervisorDelegateId);
            if (record != null && record.DelegateEmail == email && record.CandidateID == null && record.Removed == null)
            {
                return record;
            }

            return null;
        }

        public SupervisorDelegate? GetPendingSupervisorDelegateRecordByEmail(int centreId, string email)
        {
            return supervisorDelegateDataService.GetPendingSupervisorDelegateRecordByEmail(centreId, email);
        }

        public void UpdateSupervisorDelegateRecordStatus(int supervisorDelegateId, int centreId, string candidateNumber, bool setConfirmed)
        {
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(candidateNumber, centreId)!;
            var confirmed = setConfirmed ? DateTime.Now : (DateTime?)null;
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordStatus(supervisorDelegateId, delegateUser.Id, confirmed);
        }
    }
}
