namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int centreId, int supervisorDelegateId);
        SupervisorDelegate? GetPendingSupervisorDelegateRecordByIdAndEmail(int centreId, int supervisorDelegateId, string email);
    }

    public class SupervisorDelegateService : ISupervisorDelegateService
    {
        private readonly ISupervisorDelegateDataService supervisorDelegateDataService;

        public SupervisorDelegateService(ISupervisorDelegateDataService supervisorDelegateDataService)
        {
            this.supervisorDelegateDataService = supervisorDelegateDataService;
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
    }
}
