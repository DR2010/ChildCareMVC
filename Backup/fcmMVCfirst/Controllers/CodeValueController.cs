using System;
using System.Web.Mvc;
using MackkadoITFramework.ReferenceData;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Controllers
{
    public class CodeValueController : Controller
    {
        //
        // GET: /CodeValue/

        [AllowAdmin]
        public ActionResult Index(string id)
        {
            var codeValue = new CodeValue();
            codeValue.List(id);

            return View(codeValue);
        }

        //
        // GET: /CodeValue/Details/5
        [AllowAdmin]
        public ActionResult Details(CodeValue codeValue)
        {
            return View(codeValue);
        }

        //
        // GET: /CodeValue/Create

        [AllowAdmin]
        public ActionResult Create(string id)
        {
            var codeValue = new CodeValue();
            codeValue.FKCodeType = id;
            codeValue.ID = "";

            return View(codeValue);
        } 

        //
        // POST: /CodeValue/Create

        [HttpPost]
        [AllowAdmin]
        public ActionResult Create(FormCollection collection, CodeValue codeValue)
        {
            try
            {

                if (string.IsNullOrEmpty(codeValue.ID))
                {
                    // do nothing
                }
                else
                {
                    // codeType.ConnectionStringUsed = ConnectionString.GetConnectionString("makkframework");
                    codeValue.Add();
                }


                // return RedirectToAction("Index");
                return RedirectToAction("Index", new { id = codeValue.FKCodeType });
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /CodeValue/Edit/5

        [AllowAdmin]
        public ActionResult Edit(CodeValue codeValue)
        {
            return View(codeValue);
        }

        //
        // POST: /CodeValue/Edit/5

        [HttpPost]
        [AllowAdmin]
        public ActionResult Edit(CodeValue codeValue, string id)
        {
            try
            {

                var response = codeValue.Update();

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
                return RedirectToAction("Index", "CodeValue", new { id = codeValue.FKCodeType });
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An unexpected error occurred: " + ex);

                return View();
            }
        }

        //
        // GET: /CodeValue/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CodeValue/Delete/5

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
