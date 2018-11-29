using System.Web.Mvc;

namespace fcmMVCfirst.Controllers
{
    public class MaintenanceController : System.Web.Mvc.Controller
    {
        //
        // GET: /Maintenance/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ToggleCSS()
        {
            if (Session["cssRule"] != null && Session["cssRule"].ToString() == "enableCSS")
            {
                Session["cssRule"] = "disableCSS";
            }
            else
            {
                Session["cssRule"] = "enableCSS";
            }
            return View();
        }
    }
}
