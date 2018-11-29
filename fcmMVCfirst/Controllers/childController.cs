using System;
using System.Web.Mvc;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class ChildController : Controller
    {
        //
        // GET: /child/

        [AllowAdminAttribute]
        public ActionResult Index(int id = 0)
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new Child();
            ctl.List(SessionInfo.UserIDLogged, id);

            return View(ctl);
        }

        //
        // GET: /child/Details/5
        [AllowAdminAttribute]
        public ActionResult Details(int id)
        {

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ct = new Child();
            ct.UID = id;
            ct.Read();

            return View(ct);
        }

        //
        // GET: /child/Create

        [AllowAdminAttribute]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /child/Create

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Create(FormCollection collection, Child child)
        {
            try
            {
                if (string.IsNullOrEmpty(child.FirstName))
                {
                    // do nothing
                }
                else
                {
                    var response = child.Add(SessionInfo.UserIDLogged);
                    if (response.ReturnCode < 0000)
                    {
                        ModelState.AddModelError("Error", response.Message);
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
        // GET: /child/Edit/5

        [AllowAdminAttribute]
        public ActionResult Edit(int id)
        {

            var child = new Child();
            child.UID = id;
            child.Read();

            return View(child);
        }

        //
        // POST: /child/Edit/5

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Edit(Child child, int id)
        {
            try
            {
                // TODO: Add update logic here

                var response = child.Update(SessionInfo.UserIDLogged);

                if (response.ReturnCode < 0000)
                {
                    ModelState.AddModelError("Error", response.Message);
                    return View();
                }

                // 12/02/2012 - Daniel, please note the following:
                // - The syntax below takes you to the Index page, however it passes as a parameter the ID
                // which is essential to have the code value list for the previously selected Code Type
                // - I am not sure what happens when an object has to be sent back.
                //
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /child/Delete/5
        [AllowAdminAttribute]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /child/Delete/5

        [HttpPost]
        [AllowAdminAttribute]
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
