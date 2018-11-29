using System.Web.Mvc;
using FCMMySQLBusinessLibrary;
using FCMMySQLBusinessLibrary.Model.ModelMetadata;

namespace fcmMVCfirst.Controllers
{
    public class ReportMetadataController : Controller
    {
        //
        // GET: /ReportMetadata/

        public ActionResult Index()
        {
            var cvl = new ReportMetadata();
            cvl.ListDefault();

            return View(cvl);
        }

        //
        // GET: /ReportMetadata/Details/5

        public ActionResult Details(ReportMetadata reportMetadata)
        {
            return View(reportMetadata);
        }

        //
        // GET: /ReportMetadata/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ReportMetadata/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /ReportMetadata/Edit/5

        public ActionResult Edit(ReportMetadata reportMetadata)
        {
            
            return View(reportMetadata);
        }

        //
        // POST: /ReportMetadata/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ReportMetadata/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ReportMetadata/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
