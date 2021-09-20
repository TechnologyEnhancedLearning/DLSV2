namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;

    public interface IProgressDataService
    {
        void InsertNewAspProgressForTutorialIfNoneExist(int tutorialId, int customisationId);
    }

    public class ProgressDataService : IProgressDataService
    {
        private readonly IDbConnection connection;

        public ProgressDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void InsertNewAspProgressForTutorialIfNoneExist(int tutorialId, int customisationId)
        {
            connection.Execute(
                @"INSERT INTO aspProgress
                    (TutorialID, ProgressID)
                    SELECT
                        @tutorialID,
                        ProgressID
                    FROM Progress
                    WHERE RemovedDate IS NULL
                        AND CustomisationID = @customisationID
                        AND ProgressID NOT IN
                            (SELECT ProgressID
                            FROM aspProgress
                            WHERE TutorialID = @tutorialID
                                AND CustomisationID = @customisationID)",
                new { tutorialId, customisationId }
            );
        }
    }
}
