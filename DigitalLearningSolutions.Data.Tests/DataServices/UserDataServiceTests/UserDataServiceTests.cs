﻿namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        private SqlConnection connection = null!;
        private IUserDataService userDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }
    }
}
