namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Register;
    using Microsoft.Extensions.Logging;

    public interface IRegistrationService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
    }
    public class RegistrationService: IRegistrationService
    {
        private readonly IRegistrationDataService registrationDataService;
        private readonly ICryptoService cryptoService;
        private readonly ILogger<RegistrationService> logger;

        public RegistrationService(IRegistrationDataService registrationDataService,
            ICryptoService cryptoService,
            ILogger<RegistrationService> logger)
        {
            this.registrationDataService = registrationDataService;
            this.cryptoService = cryptoService;
            this.logger = logger;
        }

        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel)
        {
            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            var passwordHash = cryptoService.GetPasswordHash(delegateRegistrationModel.Password);
            registrationDataService.SetPassword(candidateNumber, passwordHash);

            return candidateNumber;
        }
    }
}
