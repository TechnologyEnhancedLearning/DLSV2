namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        private SqlConnection connection = null!;
        private TutorialContentDataService tutorialContentDataService = null!;
        private TutorialContentTestHelper tutorialContentTestHelper = null!;
        private SectionContentTestHelper sectionContentTestHelper = null!;
        private CourseContentTestHelper courseContentTestHelper = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentDataService = new TutorialContentDataService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }
    }
}
