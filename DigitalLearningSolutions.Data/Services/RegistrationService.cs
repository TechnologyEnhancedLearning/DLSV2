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
        private readonly IPasswordDataService passwordDataService;

        public RegistrationService(IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService)
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
        }

        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel)
        {
            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash);

            return candidateNumber;
        }
    }
}
