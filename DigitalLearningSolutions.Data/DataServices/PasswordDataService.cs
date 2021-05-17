namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;

    public interface IPasswordDataService
    {
        void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash);
    }

    public class PasswordDataService: IPasswordDataService
    {
        private readonly IDbConnection connection;

        public PasswordDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash)
        {
            connection.Query(
                @"UPDATE Candidates
                        SET Password = @passwordHash
                        WHERE CandidateNumber = @candidateNumber",
                new { passwordHash, candidateNumber });
        }
    }
}
