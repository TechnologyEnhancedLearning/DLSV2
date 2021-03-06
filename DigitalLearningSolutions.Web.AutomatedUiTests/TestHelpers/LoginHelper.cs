﻿namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers
{
    using OpenQA.Selenium;

    public static class LoginHelper
    {
        public static void LogUserInAsAdminAndDelegate(this IWebDriver driver, string baseUrl)
        {
            driver.Navigate().GoToUrl(baseUrl + "/Login");
            var username = driver.FindElement(By.Id("Username"));
            username.SendKeys("admin");

            var password = driver.FindElement(By.Id("Password"));
            password.SendKeys("password-1");

            var submitButton = driver.FindElement(By.TagName("form"));
            submitButton.Submit();
        }
    }
}
