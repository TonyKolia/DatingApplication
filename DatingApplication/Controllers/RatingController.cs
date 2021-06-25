using DatingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class RatingController : BaseController
    {
        [HttpPost]
        public ActionResult RateUser(string id, string rating) //action used to rate a user
        {
            var result = RatingHelper.InsertRating(int.Parse(id), int.Parse(rating)); //record and save the rating
            if (result.Success)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return Json(result); //returns to ajax

        }
    }
}