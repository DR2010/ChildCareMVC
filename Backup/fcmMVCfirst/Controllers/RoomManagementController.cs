using System;
using System.Web.Mvc;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class RoomManagementController : Controller
    {
        //
        // GET: /RoomManagement/

        [AllowAdminAttribute]
        public ActionResult Index()
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new Room();
            ctl.List(SessionInfo.UserIDLogged);

            return View(ctl);
        }

        //
        // GET: /RoomManagement/Details/5

        [AllowAdminAttribute]
        public ActionResult Details(int id)
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ct = new Room();
            ct.UID = id;
            ct.Read();

            return View();
        }

        //
        // GET: /RoomManagement/Create
        [AllowAdminAttribute]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /RoomManagement/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection, Room room)
        {
            try
            {
                if (string.IsNullOrEmpty(room.Name))
                {
                    // do nothing
                }
                else
                {
                    var response = room.Add(SessionInfo.UserIDLogged);
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
        // GET: /RoomManagement/Edit/5
 
        public ActionResult Edit(int id)
        {
            var room = new Room();
            room.UID = id;
            room.Read();

            return View(room);
        }

        //
        // POST: /RoomManagement/Edit/5

        [HttpPost]
        public ActionResult Edit(Room room, int id)
        {
            try
            {
                // TODO: Add update logic here

                var response = room.Update(SessionInfo.UserIDLogged);

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
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
        }

        //
        // GET: /RoomManagement/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RoomManagement/Delete/5

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
