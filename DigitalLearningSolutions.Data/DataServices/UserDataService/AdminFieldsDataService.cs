namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Linq;
    using Dapper;

    public partial class UserDataService
    {
        public int GetAnswerCountForCourseAdminField(int customisationId, int promptNumber)
        {
            return connection.Query<string>(
                $@"SELECT Answer{promptNumber}
                        FROM Progress
                        WHERE CustomisationID = @customisationId AND Answer{promptNumber} IS NOT NULL",
                new { customisationId }
            ).Count(x => !string.IsNullOrWhiteSpace(x));
        }

        public void DeleteAllAnswersForAdminField(int customisationId, int promptNumber)
        {
            connection.Execute(
                $@"UPDATE Progress
                        SET Answer{promptNumber} = NULL
                        WHERE CustomisationID = @customisationId",
                new { customisationId }
            );
        }
    }
}
