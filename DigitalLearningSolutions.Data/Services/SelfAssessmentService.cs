namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;

    public interface ISelfAssessmentService
    {
        string Example();
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly IDbConnection connection;

        public SelfAssessmentService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string Example()
        {
            return ""; // TODO: remove once real methods added
        }
    }
}
