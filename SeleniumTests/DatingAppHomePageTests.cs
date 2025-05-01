using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using System;
using System.IO;
using System.Linq;

namespace DatingAppUITests
{
    public class DatingAppHomePageTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            // configure selenium
            var chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--headless=new");
            chromeOptions.AddArgument("--ignore-certificate-errors");

            // get chrome driver for selenium testing.
            var driverService = ChromeDriverService.CreateDefaultService(@"D:\Uni\Courses\SIT223_ProfessionalPracticeInInformationTechnology\Tasks\7_3\SIT223-Task-7.3\SeleniumTests");
            driverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(driverService, chromeOptions);
        }

        [Test]
        public void HomePage_Should_DisplayCorrectTitle()
        {
            driver.Navigate().GoToUrl("https://localhost:7014");
            string pageTitle = driver.Title;
            Assert.IsTrue(pageTitle.Contains("Dating App"), $"Expected title to contain 'Dating App', but got '{pageTitle}'.");
        }

        [Test]
        public void HomePage_Should_HaveLoginButton()
        {
            // get for ready state and test once page rendered
            driver.Navigate().GoToUrl("https://localhost:7014");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
            var loginButton = wait.Until(drv => drv.FindElement(By.Id("loginButton")));
            Assert.IsTrue(loginButton.Displayed, "Login button should be visible on homepage.");
        }

        [Test]
        public void HomePage_Should_HaveRegisterButton()
        {
            // get for ready state and test once page rendered
            driver.Navigate().GoToUrl("https://localhost:7014");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
            var loginButton = wait.Until(drv => drv.FindElement(By.Id("registerButton")));
            Assert.IsTrue(loginButton.Displayed, "Register button should be visible on homepage.");
        }

        [Test]
        public void HomePage_Should_HaveLearnMoreButton()
        {
            // get for ready state and test once page rendered
            driver.Navigate().GoToUrl("https://localhost:7014");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
            var loginButton = wait.Until(drv => drv.FindElement(By.Id("learnMoreButton")));
            Assert.IsTrue(loginButton.Displayed, "Learn More button should be visible on homepage.");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
        }
    }
}
