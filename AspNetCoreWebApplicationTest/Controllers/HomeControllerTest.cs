using AspNetCoreWebApplication.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApplicationTest.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            HomeController controller = new HomeController();
            ViewResult result = (ViewResult) controller.Index();
            Assert.Single(result.ViewData);
            Assert.Equal("You just created a ASP.Net Core Web Application!", result.ViewData["Message"]);
        }

        [Fact]
        public void ErrorTest()
        {
            HomeController controller = new HomeController();
            ViewResult result = (ViewResult)controller.Error();
            Assert.Single(result.ViewData);
            Assert.Equal("We've encountered an error :(", result.ViewData["Message"]);
        }
    }
}