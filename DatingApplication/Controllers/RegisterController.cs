using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatingApplication.Models;
using DatingApplication.Helpers;

namespace DatingApplication.Controllers
{
    public class RegisterController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the registration page
        {
            ViewBag.Section = "Εγγραφή";
            return PartialView("_RegisterPagePartial");
        }

        [HttpPost]
        public ActionResult Index(RegisterViewModel RegisterInformation) //receives the credentials input on registration
        {
            var result = RegisterHelper.RegisterNewUser(RegisterInformation); //validate input and register user
            if(!result.Success)
            {
                ViewBag.Section = "Εγγραφή";
                ViewBag.Error = result.Message;
                return PartialView("_RegisterPagePartial");
            }

            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            if(!db.user_details.Any(u => u.user_id == userId)) //if user has no personal details, redirect to personal details page
            {
                return Redirect("~/PersonalDetails/Index");
            }

            return Redirect("~/Home/Index");
        }
    }
}