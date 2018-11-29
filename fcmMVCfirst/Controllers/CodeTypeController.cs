using System;
using System.Web.Mvc;
using MackkadoITFramework.ReferenceData;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class CodeTypeController : Controller
    {
        private bool CheckAccessToController()
        {
            if (SessionInfo.CheckIfUserHasRole("ADMIN"))
                return true;

            return false;
        }

        //
        // GET: /CodeType/

        public ActionResult Index()
        {

            if (!CheckAccessToController())
                return View("Error");

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new CodeType();
            // ctl.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
            var response = ctl.List(HeaderInfo.Instance);

            if (response.ReturnCode < 0000)
            {
                ModelState.AddModelError("Error", response.Message);
                return View("Error");
            }

            return View(ctl);
        }

        //
        // GET: /CodeType/Details/5

        public ActionResult Details(string id)
        {
            if (!CheckAccessToController())
                return View("Error");

            var codeType = new CodeType();
            // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
            codeType.Code = id;
            codeType.Read();

            return View(codeType);
        }

        //
        // GET: /CodeType/Create
        [AllowAdmin]
        public ActionResult Create()
        {

            return View();
        } 

        //
        // POST: /CodeType/Create

        [HttpPost]
        [AllowAdmin]
        public ActionResult Create(FormCollection collection, CodeType codeType)
        {

            try
            {
                // TODO: Add insert logic here

                if (string.IsNullOrEmpty( codeType.Code ))
                {
                    // do nothing
                }
                else
                {
                    // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
                    codeType.Add();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /CodeType/Edit/5
 
        public ActionResult Edit(string id)
        {
            if (!CheckAccessToController())
                return View("Error");

            var codeType = new CodeType();
            // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");

            codeType.Code = id;
            codeType.Read();

            return View(codeType);
        }

        //
        // POST: /CodeType/Edit/5

        [HttpPost]
        public ActionResult Edit(CodeType codeType)
        {
            if (!CheckAccessToController())
                return View("Error");


            try
            {
                // TODO: Add update logic here

                // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
                var response = codeType.Update();

                if (response.ReturnCode < 0000)
                {
                    ModelState.AddModelError("Error", response.Message);
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An unexpected error occurred: " + ex );

                return View();
            }
        }

        //
        // GET: /CodeType/Delete/5

        public ActionResult Delete(string id)
        {

            if (!CheckAccessToController())
                return View("Error");

            var codeType = new CodeType();
            // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");

            codeType.Code = id;
            codeType.Read();

            return View(codeType);
        }

        //
        // POST: /CodeType/Delete/5

        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {

            if (!CheckAccessToController())
                return View("Error");

            try
            {
                // TODO: Add delete logic here

                var codeType = new CodeType();
                // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");

                codeType.Code = id;
                codeType.Delete();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
