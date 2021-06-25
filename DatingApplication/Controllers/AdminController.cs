using DatingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Index() //renders the admin panel
        {
            return View("AdminPage", AdminHelper.GetAdminStats());
        }
    }
}