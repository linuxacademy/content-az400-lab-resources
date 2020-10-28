using Applitools.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace MercuryHealth.AutomatedTest.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver, Eyes eyes)
        {
            _driver = driver;
            _eyes = eyes;
        }

        #region Verifications
        public HomePage VerifyHomePageReached()
        {
            try
            {
                var homePageTitleElement = _driver.FindElement(By.Id("HomePageTitle"));
            }
            catch(Exception e)
            {
                Assert.Fail("Home page was not reached, could not find home page title: " + e.Message);
            }

            return this;
        }

        #endregion
    }
}