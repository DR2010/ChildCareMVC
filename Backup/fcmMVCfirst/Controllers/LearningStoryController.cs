using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MackkadoITFramework.ReferenceData;
using MackkadoITFramework.Utils;
using fcmMVCfirst.Common;
using fcmMVCfirst.Models;
using ReportManagement;

namespace fcmMVCfirst.Controllers
{
    public class LearningStoryController : PdfViewController
    {
        //
        // GET: /LearningStory/

        [AllowAdminAttribute]
        public ActionResult Index(string id)
        {
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ctl = new LearningStory();
            ctl.child = new Child();
            ctl.child.UID = Convert.ToInt32(id);

            ctl.child.Read();

            ctl.List(SessionInfo.UserIDLogged, ctl.child.UID);

            return View(ctl);
        }

        //
        // GET: /LearningStory/Details/5
        [AllowAdminAttribute]
        public ActionResult Details(int id)
        {   
            
            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ct = new LearningStory();
            ct.UID = id;
            ct.Read(true);

            return View(ct);
        }

        //
        // GET: /LearningStory/Print/
        [AllowAdminAttribute]
        public ActionResult Print(int id)
        {

            HeaderInfo.Instance.UserID = SessionInfo.UserIDLogged;
            HeaderInfo.Instance.CurrentDateTime = System.DateTime.Today;

            var ct = new LearningStory();
            ct.UID = id;
            ct.Read(true);

            return this.ViewPdf("", "Print", ct);

        }

        //
        // GET: /LearningStory/Create

        [AllowAdminAttribute]
        public ActionResult Create(Child child)
        {
            var ct = new LearningStory();
            ct.FKChildUID = child.UID;
            ct.child = new Child();
            ct.child.UID = child.UID;
            ct.child.Read();



            // Ler room where child is registered
            // var childRoom = new 

            return View(ct);
        } 

        //
        // POST: /LearningStory/Create

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Create(FormCollection collection, LearningStory learningStory)
        {

            try
            {
                if ( learningStory.FKChildUID <= 0
                   )
                {
                    // do nothing
                }
                else
                {

                    // Add selected items

                    AddSelectedValues(collection, learningStory, learningStory.Principles, "PRIN",4);
                    AddSelectedValues(collection, learningStory, learningStory.Practices, "PRAC",4);
                    AddSelectedValues(collection, learningStory, learningStory.LearningOutcomes, "LESI",5);

                    var lsar = new LearningStory.LearningStoryAddRequest();
                    lsar.headerInfo = HeaderInfo.Instance;
                    lsar.headerInfo.UserID = SessionInfo.UserIDLogged;

                    var response = learningStory.Add(lsar);
                    if (response.responseStatus.ReturnCode < 0000)
                    {
                        ModelState.AddModelError("Error", response.responseStatus.Message);
                        return View();
                    }
                }

                //return RedirectToAction("Index");http://localhost:51551/Child

                learningStory.child = new Child();
                learningStory.child.UID = learningStory.FKChildUID;
                learningStory.child.Read();

                return RedirectToAction("Index", new { id = learningStory.child.UID });
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                LogFile.WriteToTodaysLogFile(ex.ToString(), HeaderInfo.Instance.UserID, "", "LearningStoryController.cs");

                return View("Error");
            }
        }


        /// <summary>
        /// Add selected values
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="learningStory"></param>
        /// <param name="selectedItems"></param>
        /// <param name="type"></param>
        private static void AddSelectedValues(FormCollection collection, LearningStory learningStory, List<LearningStoryItem> selectedItems, string type, int length)
        {
            foreach (var selectedItem in collection.AllKeys)
            {
                if (selectedItem.Length < 9)
                    continue;

                if (selectedItem.Substring(4,1) != ",")
                    continue;

                if (selectedItem.Length < 5+length)
                    continue;

                bool found = false;

                var selectedItemType = selectedItem.Substring(0, 4);
                var selectedItemCode = selectedItem.Substring(5, length);

                if (selectedItemType == type)
                {
                    foreach (var existingPrinciple in learningStory.Principles)
                    {
                        if (selectedItemCode == existingPrinciple.FKCodeValue)
                        {
                            // Already included
                            found = true;
                            break;

                        }
                    }

                    if (found)
                        continue;

                    // Add item
                    //
                    selectedItems.Add(new LearningStoryItem() { FKCodeType = type, FKCodeValue = selectedItemCode });
                }
            }
        }

        /// <summary>
        /// Mark to be deleted
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="learningStory"></param>
        private static void MarkToBeDeleted(FormCollection collection, List<LearningStoryItem> learningStoryItems )
        {
            foreach (var existingPrinciple in learningStoryItems)
            {
                bool principleFound = false;

                foreach (var selectedItem in collection.AllKeys)
                {
                    if (selectedItem == existingPrinciple.FKCodeValue)
                    {
                        principleFound = true;
                        break;
                    }
                }

                // If principle is on DB but has been deselected, mark to be removed
                if (! principleFound)
                {
                    // Mark to be deleted
                    existingPrinciple.Action = "TBD";
                }
            }
        }

        //
        // GET: /LearningStory/Edit/5

        [AllowAdminAttribute]
        public ActionResult Edit(int id)
        {
            var learningStory = new LearningStory();
            learningStory.UID = id; 
            learningStory.Read(true);

            return View(learningStory);
        }

        //
        // POST: /LearningStory/Edit/5

        [HttpPost]
        [AllowAdminAttribute]
        public ActionResult Edit(FormCollection collection, LearningStory learningStory, int id)
        {
            try
            {
                // TODO: Add update logic here
                // Add selected items

                AddSelectedValues(collection, learningStory, learningStory.Principles, "PRIN", 4);
                AddSelectedValues(collection, learningStory, learningStory.Practices, "PRAC", 4);
                AddSelectedValues(collection, learningStory, learningStory.LearningOutcomes, "LESI", 5);

                var response = learningStory.Update(SessionInfo.UserIDLogged);

                if (response.ReturnCode < 0000)
                {
                    ModelState.AddModelError("Error", response.Message);
                    return View();
                }
                return RedirectToAction("Index", new { id = learningStory.FKChildUID });

                //return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /LearningStory/Delete/5

        [AllowAdminAttribute]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /LearningStory/Delete/5

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
