using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fcmMVCfirst.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Ngunnawal Child Care Center";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
