using DatingApplication.Helpers;
using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public ActionResult Index() //renders the search criteria page
        {
            ViewBag.Error = TempData["error"];
            var model = new PreferencesViewModel();
            if(Session["searchCriteria"] != null) //check if a model exists in the session (from previous failed attempt) and use that instead of empty
            {
                model = Session["searchCriteria"] as PreferencesViewModel;
            }

            model = PreferencesHelper.InitLists(model);
            model = PreferencesHelper.SetDefaults(model);
            return View("SearchCriteriaPage", model);
        }

        [HttpPost]
        public ActionResult Index(PreferencesViewModel searchCriteria) //receives the search criteria
        {
            Session["searchCriteria"] = searchCriteria; //store in session to use on home controller for rendering the results
            var result = PreferencesHelper.ValidateInputPreferences(searchCriteria); //validate the input
            if (!result.Success)
            {
                TempData["error"] = result.Message;
                return Redirect("~/Search");
            }
            return Redirect("~/Home/SearchResults");
        }
    }
}