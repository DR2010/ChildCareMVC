using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MackkadoITFramework.Security;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class SecurityUserController : Controller
    {

        //
        // GET: /SecurityUser/
        [AllowAdmin]
        public ActionResult Index()
        {

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new UserAccess();
            // ctl.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
            ctl.ListUsers();

            return View(ctl);
        }

        //
        // GET: /SecurityUser/Details/5

        [AllowAdmin]
        public ActionResult Details(UserAccess userAccess)
        {
            return View(userAccess);
        }

        //
        // GET: /SecurityUser/Create

        public ActionResult Create(string id)
        {
            var userAccess = new UserAccess();
            userAccess.UserID = id;

            return View(userAccess);
        } 

        //
        // POST: /SecurityUser/Create

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
        // GET: /SecurityUser/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SecurityUser/Edit/5

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
        // GET: /SecurityUser/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /SecurityUser/Delete/5

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
