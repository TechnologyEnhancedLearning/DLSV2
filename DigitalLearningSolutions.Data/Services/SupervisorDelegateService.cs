namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId);
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
    }
}
