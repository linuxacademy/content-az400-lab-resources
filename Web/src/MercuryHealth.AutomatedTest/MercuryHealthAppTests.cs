using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MercuryHealth.AutomatedTest.Pages;

namespace MercuryHealth.AutomatedTest
{
    [TestClass]
    public class MercuryHealthAppTests
    {
        private static string _homePageUrl;
        private static string _browserType;
        private static string _applitoolsApiKey;
        private static string _applitoolsBatchName;
        private static string _applitoolsBatchId;

        public MercuryHealthAppTests()
        {

        }

        #region Setup and teardown
        [ClassInitialize]

        public static void Initialize(TestContext context)
        {
            try
            {
                _browserType = context.Properties["browserType"].ToString();
                _homePageUrl = context.Properties["appUrl"].ToString();
                _applitoolsApiKey = context.Properties["applitoolsKey"].ToString();

            }
            catch (Exception)
            {
                _browserType = "chrome";
                _homePageUrl = "https://mercuryhealth-dev.azurewebsites.net/";
                _applitoolsApiKey = "3P7MdEi4EmMpT7mXOKAACHNYA11082kFXHWYTqNUMP4Ys110";
            }

            // obtain batch name and ID from the environment variables
            _applitoolsBatchName = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_NAME");
            _applitoolsBatchId = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_ID");
        }

        [ClassCleanup]
        public static void Cleanup()
        {

        }
        #endregion

        #region Tests
        [TestMethod]
        [TestCategory("UITests")]
        public void BrowseToHomePageTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey, 
                                         _applitoolsBatchName,
                                         _applitoolsBatchId, 
                                         "Browse To Home Page Test", 
                                         _browserType);

            try
            {
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page");
            }
            catch (Exception e)
            {
                Assert.Fail("Execption occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }

        }

        [TestMethod]
        [TestCategory("UITests")]
        public void BrowseToNutritionPageTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                        _applitoolsBatchName,
                                        _applitoolsBatchId,
                                        "Browse To Nutrition Page Test",
                                        _browserType);

            try
            {
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page")
                    .ClickNutritionLink()
                    .VerifyNutritionPageReached()
                    .TakeVisualPicture<NutritionPage>("Nutrition Page");
            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured durring test run: " + e);
            }
            finally
            {
                webApp.Close();
            }
        }

        [TestMethod]
        [TestCategory("UITests")]
        public void BrowseToCreateFoodLogEntryTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Browse To Create Food Log Entry Test",
                                         _browserType);

            try
            {
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page")
                    .ClickNutritionLink()
                    .VerifyNutritionPageReached()
                    .TakeVisualPicture<NutritionPage>("Nutrition Page")
                    .ClickCreateNewLink()
                    .VerifyCreatePageReached()
                    .TakeVisualPicture<CreatePage>("Create Food Log Page");
            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }
        }

        [TestMethod]
        [TestCategory("UITests")]
        public void Add1stDonutTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Add 1st Donut Test",
                                         _browserType);

            try
            {
                // browse to home page of app
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page")

                    //go to nutrition page
                    .ClickNutritionLink()
                    .VerifyNutritionPageReached()

                    // clean up and delete all donuts, take a picture of cleaned up nutrition page
                    .RemoveAllFood("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page")

                    // click create new link to add new food
                    .ClickCreateNewLink()
                    .VerifyCreatePageReached()
                    .TakeVisualPicture<CreatePage>("Create Page")

                    // add donut as a food item and click the add button
                    .SetDescription("Donut")
                    .ClickCreateButton()
                    .VerifyNutritionPageReached()
                    .VerifyFoodInTable("Donut", 1)
                    .TakeVisualPicture<NutritionPage>("Nutrition Page After Adding Donut")

                    // clean up and delete all donuts 
                    .RemoveAllFood("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Paqge After Removing All Donuts");
            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }
        }

        [TestMethod]
        [TestCategory("UITests")]
        public void DeleteDonutTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Delete Donut Test",
                                         _browserType);

            try
            {
                // browse to home page of app
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page")

                    //go to nutrition page
                    .ClickNutritionLink()
                    .VerifyNutritionPageReached()

                    // clean up and delete all donuts 
                    .RemoveAllFood("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page After Removing All Donuts")

                    // click create new link to add new food
                    .ClickCreateNewLink()
                    .VerifyCreatePageReached()
                    .TakeVisualPicture<CreatePage>("Create Page")

                    // add donut as a food item and click the add button
                    .SetDescription("Donut")
                    .ClickCreateButton()
                    .VerifyNutritionPageReached()
                    .VerifyFoodInTable("Donut", 1)
                    .TakeVisualPicture<NutritionPage>("Nutrition Page with 1 Donut")

                    // delete the donut
                    .ClickDeleteFood("Donut")
                    .VerifyDeleteFoodPageReached()
                    .ClickDeleteButton()
                    .VerifyNutritionPageReached()
                    .VerifyFoodNotInTable("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page After Delete Donut");

            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }
        }

        [TestMethod]
        [TestCategory("UITests")]
        public void EditDonutTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Edit Donut Test",
                                         _browserType);

            try
            {
                // browse to home page of app
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Page")

                    //go to nutrition page
                    .ClickNutritionLink()
                    .VerifyNutritionPageReached()

                    // clean up and delete all donuts 
                    .RemoveAllFood("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page after removing all donuts")

                    // click create new link to add new food
                    .ClickCreateNewLink()
                    .VerifyCreatePageReached()
                    .TakeVisualPicture<CreatePage>("Create Page")

                    // add donut as a food item and click the add button
                    .SetDescription("Donut")
                    .ClickCreateButton()
                    .VerifyNutritionPageReached()
                    .VerifyFoodInTable("Donut", 1)
                    .TakeVisualPicture<NutritionPage>("NutritionPage after adding a donut")

                    // click on the edit for the donut
                    .ClickEditFoodLink("Donut")
                    .VerifyEditFoodPageReached()
                    .TakeVisualPicture<EditFoodPage>("Edit Food page")
                    .SetCarbs("999.99")
                    .ClickSaveButton()
                    .VerifyNutritionPageReached()
                    .VerifyCarbs("Donut", "999.99")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page after setting carbs to 999.99")

                    // clean up and delete all donuts 
                    .RemoveAllFood("Donut")
                    .TakeVisualPicture<NutritionPage>("Nutrition Page after removing all donuts");


            }
            catch (Exception e)
            {
                Assert.Fail("Execption occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }

        }

        [TestMethod]
        [TestCategory("UITests")]
        public void BrowseToExercisePageTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Browse To Exercise Page Test",
                                         _browserType);

            try
            {
                webApp.BrowseToHomePage(_homePageUrl)
                .VerifyHomePageReached()
                .TakeVisualPicture<HomePage>("Home Page")
                .ClickExercisesLink()
                .VerifyExercisePageReached()
                .TakeVisualPicture<ExercisePage>("Exercise Page");

            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured during test run: " + e);
            }
            finally
            {
                webApp.Close();
            }


        }

        [TestMethod]
        [TestCategory("UITests")]
        public void BrowseToCreateExerciseEntryTest()
        {
            var webApp = HomePage.Launch(_applitoolsApiKey,
                                         _applitoolsBatchName,
                                         _applitoolsBatchId,
                                         "Browse To Create Exercise Entry Test",
                                         _browserType);

            try
            {
                webApp.BrowseToHomePage(_homePageUrl)
                    .VerifyHomePageReached()
                    .TakeVisualPicture<HomePage>("Home Pgae")
                    .ClickExercisesLink()
                    .VerifyExercisePageReached()
                    .TakeVisualPicture<ExercisePage>("Exercise Page")
                    .ClickCreateNewLink()
                    .VerifyCreatePageReached()
                    .TakeVisualPicture<CreatePage>("Create Page");

            }
            catch (Exception e)
            {
                Assert.Fail("Exception occured during test run : " + e);
            }
            finally
            {
                webApp.Close();
            }
        }


        //[TestMethod]
        //[TestCategory("UITestsBroken")]
        //public void BrowseToMyMetricsPageTest()
        //{
        //    _homePage.BrowseToHomePage(_homePageUrl)
        //        .VerifyHomePageReached()
        //        .ClickMyMetricsLink()
        //        .VerifyMyMeticsPageReached();

        //}

        //[TestMethod]
        ////[TestCategory("UITestsBroken")]
        //public void Add2ndDonutTest()
        //{
        //    // browse to home page of app
        //    _homePage.BrowseToHomePage(_homePageUrl)
        //        .VerifyHomePageReached()

        //        //go to nutrition page
        //        .ClickNutritionLink()
        //        .VerifyNutritionPageReached()

        //        // clean up and delete all donuts 
        //        .RemoveAllFood("Donut")

        //        // click create new link to add new food
        //        .ClickCreateNewLink()
        //        .VerifyCreatePageReached()

        //        // add donut as a food item and click the add button
        //        .SetDescription("Donut")
        //        .ClickCreateButton()
        //        .VerifyNutritionPageReached()
        //        .VerifyFoodInTable("Donut", 1)

        //        // add the second donut
        //        .ClickCreateNewLink()
        //        .VerifyCreatePageReached()
        //        .SetDescription("Donut")
        //        .ClickCreateButton()
        //        .VerifyNutritionPageReached()
        //        .VerifyFoodInTable("Donut", 2);

        //}

        #endregion
    }
}
