using System.Web.Mvc;

namespace fcmMVCfirst.ReferenceData.Controller
{
    public class ReferenceDataController : System.Web.Mvc.Controller
    {
        //
        // GET: /ReferenceData/

        public ActionResult Index()
        {
            return View("~/ReferenceData/View/Index.cshtml");
        }

    }
}
