namespace DigitalLearningSolutions.Data.Tests.Services.TutorialContentServiceTests
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;

    internal partial class TutorialContentServiceTests
    {
        private TutorialContentService tutorialContentService;
        private TutorialContentTestHelper tutorialContentTestHelper;
        private SectionContentTestHelper sectionContentTestHelper;
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentService = new TutorialContentService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }
    }
}
