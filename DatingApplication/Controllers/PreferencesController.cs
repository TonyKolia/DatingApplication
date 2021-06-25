using DatingApplication.Helpers;
using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class PreferencesController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the preferences page on registration
        {
            var model = PreferencesHelper.InitLists(new PreferencesViewModel());
            ViewBag.action = "create"; //action is used in view to post form in the correct action depending on user location
            return View("PreferencesPage", PreferencesHelper.SetDefaults(model));
        }

        [HttpPost]
        public ActionResult Index(PreferencesViewModel preferences) //receives the preferences input on registration
        {
            var result = PreferencesHelper.ValidateInputPreferences(preferences);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.action = "create"; //action is used in view to post form in the correct action depending on user location
                return View("PreferencesPage", PreferencesHelper.InitLists(preferences));
            }

            result = PreferencesHelper.InsertPreferences(preferences); //validate and insert input
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.action = "create"; //action is used in view to post form in the correct action depending on user location
                return View("PreferencesPage", PreferencesHelper.InitLists(preferences));
            }

            return Redirect("~/Home"); //if success, redirect to home. Registration proccess completed
        }

        [HttpGet]
        public ActionResult Edit() //renders the preferences page inside the application
        {
            if(CommonHelpers.GetLoggedUserInfo().Category != 2) //premium only feature, redirect if simple user
            {
                return Redirect("~/Home");
            }

            ViewBag.action = "edit"; //action is used in view to post form in the correct action depending on user location
            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];
            return PartialView("PreferencesPage", PreferencesHelper.GetPreferences());
        }

        [HttpPost]
        public ActionResult Edit(PreferencesViewModel preferences) //receives the preferences input inside the application
        {
            var result = PreferencesHelper.ValidateInputPreferences(preferences);
            if (!result.Success)
            {
                TempData["error"] = result.Message;
                ViewBag.action = "create"; //action is used in view to post form in the correct action depending on user location
                return Redirect("~/Preferences/Edit");
            }
            result = PreferencesHelper.UpdatePreferences(preferences); //validate and update 
            if (!result.Success)
            {
                TempData["error"] = result.Message;
            }
            else
            {
                TempData["success"] = result.Message;
            }

            return Redirect("~/Preferences/Edit");
        }
    }
}