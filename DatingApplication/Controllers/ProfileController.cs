using DatingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class ProfileController : BaseController
    {
        [HttpGet]
        public ActionResult Index(int id) //renders the selected user profile
        {
            //logged in user cant see his own profile. also premium feature only, redirect if simple user
            if(id == CommonHelpers.GetLoggedUserInfo().Id || CommonHelpers.GetLoggedUserInfo().Category == 1)
            {
                return Redirect("~/Home");
            }

            ProfileHelper.MarkProfileVisit(id); //record and save the profile visit

            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];

            return PartialView("ProfilePage",ProfileHelper.GetProfileData(id));
        }
    }
}