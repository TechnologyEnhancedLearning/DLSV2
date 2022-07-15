namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Auth;

    public interface IRegistrationConfirmationDataService
    {
        void SetRegistrationConfirmation(RegistrationConfirmationModel model);
    }

    public class RegistrationConfirmationDataService : IRegistrationConfirmationDataService
    {
        private readonly IDbConnection connection;

        public RegistrationConfirmationDataService(
            IDbConnection connection
        )
        {
            this.connection = connection;
        }

        public void SetRegistrationConfirmation(RegistrationConfirmationModel model)
        {
            connection.Execute(
                @"
                    UPDATE DelegateAccounts
                    SET RegistrationConfirmationHash = @Hash,
                        RegistrationConfirmationHashCreationDateTime = @CreateTime
                    WHERE ID = @DelegateId
                ",
                new
                {
                    model.Hash,
                    model.CreateTime,
                    model.DelegateId,
                }
            );
        }
    }
}
