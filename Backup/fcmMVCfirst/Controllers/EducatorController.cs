using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class EducatorController : Controller
    {
        //
        // GET: /Worker/

        [AllowAdmin]
        public ActionResult Index()
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new Educator();
            ctl.List(SessionInfo.UserIDLogged);

            return View(ctl);
        }

        //
        // GET: /Worker/Details/5

        public ActionResult Details(int id)
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ct = new Educator();
            ct.UID = id;
            ct.Read();

            return View(ct);
        }

        //
        // GET: /Worker/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Worker/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection, Educator educator)
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = DateTime.Today;

            try
            {
                if (string.IsNullOrEmpty(educator.FirstName))
                {
                    // do nothing
                }
                else
                {
                    // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
                    
                    var workerAddRequest = new WorkerAddRequest();
                    workerAddRequest.XHeaderInfo = HeaderInfo.Instance;
                    workerAddRequest.XEducator = educator;

                    var workAddResponse = educator.Add(workerAddRequest);

                    if (workAddResponse.XResponseStatus.ReturnCode < 0000)
                    {
                        ModelState.AddModelError("Error", workAddResponse.XResponseStatus.Message);
                        return View();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Worker/Edit/5
 
        public ActionResult Edit(int id)
        {

            var worker = new Educator();
            worker.UID = id;
            worker.Read();

            return View(worker);
        }

        //
        // POST: /Worker/Edit/5

        [HttpPost]
        public ActionResult Edit(Educator educator, int id)
        {
            try
            {
                var workerUpdateRequest = new WorkerUpdateRequest();
                workerUpdateRequest.XHeaderInfo = HeaderInfo.Instance;
                workerUpdateRequest.XEducator = educator;

                var response = educator.Update(workerUpdateRequest);

                if (response.XResponseStatus.ReturnCode < 0000)
                {
                    ModelState.AddModelError("Error", response.XResponseStatus.Message);
                    return View();
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Worker/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Worker/Delete/5

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
