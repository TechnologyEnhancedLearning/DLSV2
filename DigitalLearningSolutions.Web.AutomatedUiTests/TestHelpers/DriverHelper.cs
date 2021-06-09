namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;

    public static class DriverHelper
    {
        public static ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }

        public static void FillTextInput(this IWebDriver driver, string inputId, string inputText)
        {
            var answer = driver.FindElement(By.Id(inputId));
            answer.SendKeys(inputText);
        }

        public static void ClickButtonByText(this IWebDriver driver, string text)
        {
            var addButton = driver.FindElement(By.XPath($"//button[.='{text}']"));
            addButton.Click();
        }

        public static void SelectDropdownItemValue(this IWebDriver driver, string dropdownId, string selectedValue)
        {
            var dropdown = new SelectElement(driver.FindElement(By.Id(dropdownId)));
            dropdown.SelectByValue(selectedValue);
        }

        public static void SubmitForm(this IWebDriver driver)
        {
            var selectPromptForm = driver.FindElement(By.TagName("form"));
            selectPromptForm.Submit();
        }
    }
}
