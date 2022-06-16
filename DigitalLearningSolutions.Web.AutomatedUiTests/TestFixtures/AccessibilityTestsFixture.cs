namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures
{
    using System;
    using System.Data.SqlClient;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;

    public class AccessibilityTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        internal readonly string BaseUrl;
        internal readonly IWebDriver Driver;
        private readonly string connectionString;
        private readonly SeleniumServerFactory<TStartup> factory;

        public AccessibilityTestsFixture()
        {
            factory = new SeleniumServerFactory<TStartup>();
            BaseUrl = factory.RootUri;
            connectionString = factory.ConnectionString;
            Driver = DriverHelper.CreateHeadlessChromeDriver();
        }

        public void Dispose()
        {
            ClearMultiPageFormData();
            Driver.Quit();
            Driver.Dispose();
            factory.Dispose();
        }

        private void ClearMultiPageFormData()
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("DELETE FROM MultiPageFormData", connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
