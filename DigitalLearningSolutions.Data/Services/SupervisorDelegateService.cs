namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId);
        SupervisorDelegate? GetPendingSupervisorDelegateRecordByIdAndEmail(int supervisorDelegateId, string email);
    }

    public class SupervisorDelegateService : ISupervisorDelegateService
    {
        private readonly ISupervisorDelegateDataService supervisorDelegateDataService;

        public SupervisorDelegateService(ISupervisorDelegateDataService supervisorDelegateDataService)
        {
            this.supervisorDelegateDataService = supervisorDelegateDataService;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId)
        {
            return supervisorDelegateDataService.GetSupervisorDelegateRecord(supervisorDelegateId);
        }

        public SupervisorDelegate? GetPendingSupervisorDelegateRecordByIdAndEmail(
            int supervisorDelegateId,
            string email
        )
        {
            var record = GetSupervisorDelegateRecord(supervisorDelegateId);
            if (record != null && record.DelegateEmail == email && record.CandidateID == null && record.Removed == null)
            {
                return record;
            }

            return null;
        }

    }
}
