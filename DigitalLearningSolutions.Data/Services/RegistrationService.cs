namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Register;
    using Microsoft.Extensions.Logging;

    public interface IRegistrationService
    {
        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
    }
    public class RegistrationService: IRegistrationService
    {
        private readonly IRegistrationDataService registrationDataService;
        private readonly ILogger<RegistrationService> logger;

        public RegistrationService(IRegistrationDataService registrationDataService,
            ILogger<RegistrationService> logger)
        {
            this.registrationDataService = registrationDataService;
            this.logger = logger;
        }

        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel)
        {
            return registrationDataService.RegisterDelegate(delegateRegistrationModel);
        }
    }
}
