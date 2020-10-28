using Applitools.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace MercuryHealth.AutomatedTest.Pages
{
    public class DeleteFoodPage : BasePage
    {
        public DeleteFoodPage(IWebDriver driver, Eyes eyes)
        {
            _driver = driver;
            _eyes = eyes;
        }

        #region Actions
        public NutritionPage ClickDeleteButton()
        {
            try
            {
                var deleteButton = _driver.FindElement(By.ClassName("btn"));
                deleteButton.Click();
            }
            catch(Exception e)
            {
                Assert.Fail("Delete food failed: " + e.Message);
            }
            return new NutritionPage(_driver, _eyes);
        }
        #endregion
        
        #region Verification
        public DeleteFoodPage VerifyDeleteFoodPageReached()
        {
            try
            {
                var deleteHeader = _driver.FindElement(By.XPath("/html/body/div[2]/h2"));
            }
            catch(Exception e)
            {
                Assert.Fail("Delete page is not reached: " + e.Message);
            }
            return this;
        }
        #endregion
    }
}