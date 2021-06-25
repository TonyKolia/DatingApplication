using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatingApplication.Helpers;
using DatingApplication.Models;

namespace DatingApplication.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the collection of cards representing the user matches
        {
            var userInfo = CommonHelpers.GetLoggedUserInfo();
            if(userInfo is null)
            {
                return Redirect("~/Register/Index");
            }

            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];

            return View(HomeHelper.GetUserMatches());
        }

        [HttpGet]
        public ActionResult ProfileVisitors() //renders the collection of cards representing the user visitors
        {
            if(CommonHelpers.GetLoggedUserInfo().Category != 2) //premium only, redirect simple user
            {
                return Redirect("~/Home");
            }
            return View("Index", HomeHelper.GetProfilesVistedUsers());
        }

        [HttpGet]
        public ActionResult ProfileRaters() //renders the collection of cards representing the user raters
        {
            if (CommonHelpers.GetLoggedUserInfo().Category != 2) //premium only, redirect simple user
            {
                return Redirect("~/Home");
            }
            return View("Index", HomeHelper.GetProfileRaters());
        }

        [HttpGet]
        public ActionResult AllUsers() //renders the collection of cards representing all the users
        {
            if (CommonHelpers.GetLoggedUserInfo().Category != 2) //premium only, redirect simple user
            {
                return Redirect("~/Home");
            }
            return View("Index", HomeHelper.GetAllUsers());
        }

        [HttpGet]
        public ActionResult SearchResults() //renders the collection of cards representing the users returned from search
        {
            if(Session["searchCriteria"] != null)
            {
                var searchCriteria = Session["searchCriteria"] as PreferencesViewModel; //get the viewmodel from session where it was saved by the search based on the criteria
                Session["searchCriteria"] = null; //clear the session
                return View("Index", HomeHelper.GetUserMatches(searchCriteria, true));
            }
            else
            {
                TempData["error"] = "Σφάλμα κατά την αναζήτηση.";
                return Redirect("~/Search");
            }
        }
    }
}