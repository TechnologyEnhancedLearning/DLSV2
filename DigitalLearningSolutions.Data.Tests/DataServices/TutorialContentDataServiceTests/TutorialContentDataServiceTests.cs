namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        private TutorialContentDataService tutorialContentDataService;
        private TutorialContentTestHelper tutorialContentTestHelper;
        private SectionContentTestHelper sectionContentTestHelper;
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentDataService = new TutorialContentDataService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }
    }
}
