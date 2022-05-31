namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Linq;
    using Dapper;

    public partial class UserDataService
    {
        public void UpdateDelegateUserCentrePrompts(
            int id,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            DateTime detailsLastChecked
        )
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                        SET
                            Answer1 = @answer1,
                            Answer2 = @answer2,
                            Answer3 = @answer3,
                            Answer4 = @answer4,
                            Answer5 = @answer5,
                            Answer6 = @answer6,
                            CentreSpecificDetailsLastChecked = @detailsLastChecked
                        WHERE ID = @id",
                new { answer1, answer2, answer3, answer4, answer5, answer6, id, detailsLastChecked }
            );
        }

        public int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber)
        {
            return connection.Query<string>(
                $@"SELECT Answer{promptNumber}
                        FROM Candidates
                        WHERE CentreID = @centreId AND Answer{promptNumber} IS NOT NULL",
                new { centreId }
            ).Count(x => !string.IsNullOrWhiteSpace(x));
        }

        public void DeleteAllAnswersForPrompt(int centreId, int promptNumber)
        {
            connection.Execute(
                $@"UPDATE Candidates
                        SET Answer{promptNumber} = NULL
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }
    }
}
