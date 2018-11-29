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
    public class SecurityUserRoleController : Controller
    {

        //
        // GET: /SecurityUserRole/

        [AllowAdminAttribute]
        public ActionResult Index(string id) // UserAccess userAccess
        {

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            ConnString.ConnectionStringFramework = ConnectionString.GetConnectionString("makkframework");
           
            SecurityUserRole userRole = new SecurityUserRole(HeaderInfo.Instance);
            userRole.UserRoleList(id);
            userRole.FK_UserID = id;

            return View( userRole );
        }

        //
        // GET: /SecurityUserRole/Details/5

        [AllowAdminAttribute]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SecurityUserRole/Create

        public ActionResult Create(string id)
        {

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var securityUserRole = new SecurityUserRole(HeaderInfo.Instance);
            securityUserRole.FK_UserID = id;

            return View(securityUserRole);
        } 

        //
        // POST: /SecurityUserRole/Create

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Create(FormCollection collection, SecurityUserRole securityUserRole)
        {
            try
            {
                if (string.IsNullOrEmpty(securityUserRole.FK_UserID))
                {
                    // do nothing
                }
                else
                {

                    var response = securityUserRole.Add();
                    if (response.ReturnCode < 0000)
                    {
                        ModelState.AddModelError("Error", response.Message);
                        return View();
                    }
                }

                return RedirectToAction("Index", new { id = securityUserRole.FK_UserID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                return View();
            }
        }
        
        //
        // GET: /SecurityUserRole/Edit/5

        [AllowAdminAttribute]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SecurityUserRole/Edit/5

        [HttpPost]
        [AllowAdminAttribute]
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
        // GET: /SecurityUserRole/Delete/5

        [AllowAdminAttribute]
        public ActionResult Delete(SecurityUserRole item)
        {


            return View(item);

        }

        //
        // POST: /SecurityUserRole/Delete/5

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Delete(SecurityUserRole item, FormCollection collection)
        {
            try
            {
                // Update database
                //
                SecurityUserRole newUserRole = new SecurityUserRole(HeaderInfo.Instance);
                newUserRole.UniqueID = item.UniqueID;
                newUserRole.FK_Role = item.FK_Role;
                newUserRole.FK_UserID = item.FK_UserID;

                var response = newUserRole.Delete();

                if (response.ReturnCode == 0001 && response.ReasonCode == 0001)
                {
                    return RedirectToAction("Index", new { id = item.FK_UserID });
                }
                else
                {
                    ModelState.AddModelError("Error", response.Message);
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Index", new { id = item.FK_UserID });
            }


        }
    }
}
