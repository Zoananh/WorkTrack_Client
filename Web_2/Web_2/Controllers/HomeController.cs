using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Maininfo()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Installedapp()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}