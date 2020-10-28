#region Namespaces
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
#endregion

namespace MercuryHealth.Web.Controllers
{
    
    public class HomeController : Controller
    {

       
        public ActionResult Index()
        {
            ViewBag.Message = "Mercury Health Group";
            ViewBag.Environment = ConfigurationManager.AppSettings["Environment"].ToString();

            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your about page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }

}