using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class LogoutController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //logout action, clear the session that hold the logged in user
        {
            Session.Abandon();
            return Redirect("Register/Index");
        }
    }
}