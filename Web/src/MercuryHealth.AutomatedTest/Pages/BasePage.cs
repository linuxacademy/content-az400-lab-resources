using Applitools;
using Applitools.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercuryHealth.AutomatedTest.Pages
{
    public class BasePage
    {
        protected IWebDriver _driver;
        protected Eyes _eyes;

        private static string TESTNAME;
        private static Size WINDOWSIZE = new Size(1366, 768);


        protected BasePage()
        {

        }


        #region Actions
        public void Close()
        {
            try
            {
                // end the applitools test
                _eyes.Close();
            }
            catch(Exception e)
            {
                _eyes.AbortIfNotClosed();
                Assert.Inconclusive("Inconclusive result. Page image differences detected when closing eyes: " + e);
            }
            //_driver.Close();
            _driver.Quit();
            _driver.Dispose();
        }

        public void AbortEyesIfNotClosed()
        {
            _eyes.AbortIfNotClosed();
        }

        public HomePage BrowseToHomePage(string homePageUrl)
        {
            // browse to the home page
            _driver.Navigate().GoToUrl(homePageUrl);
            return new HomePage(_driver, _eyes);
        }

        public NutritionPage ClickNutritionLink()
        {
            try
            {
                var nutritionLink = _driver.FindElement(By.LinkText("Nutrition"));
                nutritionLink.Click();
            }
            catch (Exception e)
            {
                Assert.Fail("Could not find nutrition link: " + e.Message);
            }
            return new NutritionPage(_driver, _eyes);
        }

        public ExercisePage ClickExercisesLink()
        {
            try
            {
                var nutritionLink = _driver.FindElement(By.LinkText("Exercises"));
                nutritionLink.Click();
            }
            catch (Exception e)
            {
                Assert.Fail("Could not find exercises link: " + e.Message);
            }
            return new ExercisePage(_driver, _eyes);
        }

        public MyMetricsPage ClickMyMetricsLink()
        {
            try
            {
                var nutritionLink = _driver.FindElement(By.LinkText("My Metrics"));
                nutritionLink.Click();
            }
            catch (Exception e)
            {
                Assert.Fail("Could not find my metrics link: " + e.Message);
            }
            return new MyMetricsPage(_driver, _eyes);
        }

        #endregion

        #region Applitools Actions
        public T TakeVisualPicture<T>(string tag)
        {
  
            _eyes.CheckWindow(tag);
            return (T) Convert.ChangeType(this, typeof(T));
        }
        #endregion

        #region Launch selenium web driver
        public static HomePage Launch(string appliToolsKey, string applitoolsBatchName, string applitoolsBatchId, string testName, string browser = "ie")
        {
            // based on the browser passed in, created your web driver
            IWebDriver driver;
            if (browser.Equals("chrome"))
            {
                driver = new ChromeDriver();
            }
            else
            {
                driver = new InternetExplorerDriver();
            }

            // obtain the batch name and ID from the environment variables
            var batchName = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_NAME");
            var batchId = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_ID");

            // set the batch
            var batchInfo = new BatchInfo(batchName);
            batchInfo.Id = batchId;


            // initialize the eyes SDK and set your private API key
            var eyes = new Eyes();
            //eyes.ApiKey = appliToolsKey;
            //eyes.Batch = batchInfo;

            //// Start the test by setting AUT's name, window or the page name that's being tested, 
            ////viewport width and height
            //TESTNAME = testName;
            //eyes.Open(driver, "Mercury Health Web App", TESTNAME, WINDOWSIZE);

            // set the window size of the browser and browse to the home page
            driver.Manage().Window.Size = WINDOWSIZE;
            return new HomePage(driver, eyes);
        }
        #endregion

    }
}
