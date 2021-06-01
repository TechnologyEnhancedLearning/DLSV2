namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers
{
    using OpenQA.Selenium.Chrome;

    public static class DriverHelper
    {
        public static ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }
    }
}
