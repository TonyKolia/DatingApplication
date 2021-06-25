using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class BaseController : Controller
    {
        //test

        protected DatingEntities db = null;

        public BaseController() //used just to provide db context to controllers easily
        {
            db = new DatingEntities();
        }

    }
}