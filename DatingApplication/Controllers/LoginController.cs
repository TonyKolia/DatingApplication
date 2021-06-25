using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatingApplication.Models;
using DatingApplication.Helpers;
using Microsoft.Ajax.Utilities;

namespace DatingApplication.Controllers
{
    public class LoginController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the login page
        {
            ViewBag.Section = "Σύνδεση";
            return View("_LoginPagePartial");
        }

        [HttpPost]
        public ActionResult Index(users LoginInfo) //receives the login credentials
        {
            var result = LoginHelper.LoginUser(LoginInfo);
            if(!result.Success)
            {
                ViewBag.Section = "Σύνδεση";
                ViewBag.Error = result.Message;
                return View("_LoginPagePartial");
            }

            if(CommonHelpers.GetLoggedUserInfo().Category == 3) //redirect admin straight to admin panel
            {
                return Redirect("~/Admin/Index");
            }

            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            if (!db.user_details.Any(u => u.user_id == userId)) //if user has no personal details, redirect to the personal details page
            {
                return Redirect("~/PersonalDetails/Index");
            }

            if(!db.user_images.Any(u => u.user_id == userId)) //if user has no images, redirect to the images page
            {
                return Redirect("~/PersonalDetails/UploadImages");//?action='create'");
            }

            //if user has no preferences, redirect to the preferences page (hobbies are optional)
            if(!db.user_preferences_age.Any(u => u.user_id == userId) && !db.user_preferences_eye_color.Any(u => u.user_id == userId) &&
               !db.user_preferences_gender.Any(u => u.user_id == userId) && !db.user_preferences_hair_color.Any(u => u.user_id == userId) &&
               !db.user_preferences_height.Any(u => u.user_id == userId) && !db.user_preferences_weight.Any(u => u.user_id == userId))
            {
                return Redirect("~/Preferences");
            }

            return Redirect("~/Home/Index"); //successful login, redirect to home
        }
    }
}