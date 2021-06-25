using DatingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class DownloadController : BaseController
    {
        //render the donwload page
        [HttpGet]
        public ActionResult Index()
        {
            if (CommonHelpers.GetLoggedUserInfo().Category == 1)
            {
                return Redirect("~/Home");
            }

            ViewBag.Success = TempData["success"];
            ViewBag.Error = TempData["error"];

            return View("DownloadPage");
        }

        [HttpPost]
        public ActionResult DownloadApp() //controller used to download the "mobile app"
        {
            if (CommonHelpers.GetLoggedUserInfo().Category == 1)
            {
                return Redirect("~/Home");
            }

            try
            {
                string Path = Url.Content(Server.MapPath("~/Content/Images/1.jpg")); //get the file
                string filename = "NewBeginnings.apk";
                byte[] fileBytes = System.IO.File.ReadAllBytes(Path);
                return File(fileBytes, "application/force-download", filename); //return file result for download
            }
            catch(Exception ex)
            {
                TempData["error"] = "Σφάλμα κατά τη λήψη.";
                return Redirect("~/Download");
            }
        }
    }
}