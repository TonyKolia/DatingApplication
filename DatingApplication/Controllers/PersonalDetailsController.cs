using DatingApplication.Helpers;
using DatingApplication.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class PersonalDetailsController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the personal details page after registration
        {
            var model = new PersonalDetailsViewModel();
            ViewBag.action = "create"; //action is used in view to post form in the correct action depending on user location
            return View("PersonalDetailsPage", PersonalDetailsHelper.InitLists(model));
        }

        [HttpPost]
        public ActionResult Index(PersonalDetailsViewModel personalDetails) //receives the personal details after registration
        {
            var result = PersonalDetailsHelper.InsertPersonalDetails(personalDetails); //validate and save input
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.action = "create";
                return View("PersonalDetailsPage", PersonalDetailsHelper.InitLists(personalDetails));
            }

            return Redirect("~/PersonalDetails/UploadImages"); //if successful, redirect to image page 
        }

        [HttpGet]
        public ActionResult Edit() //renders the personal details page where user can edit them in the application
        {
            ViewBag.action = "edit"; //action is used in view to post form in the correct action depending on user location
            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];
            return PartialView("PersonalDetailsPage", PersonalDetailsHelper.GetPersonalDetails());
        }

        [HttpPost]
        public ActionResult Edit(PersonalDetailsViewModel personalDetailsViewModel) //receives the personal details on edit
        {
            var result = PersonalDetailsHelper.UpdatePersonalDetails(personalDetailsViewModel); //validate and update user preferences based on input
            if (!result.Success)
            {
                TempData["error"] = result.Message;
            }
            else
            {
                TempData["success"] = result.Message;
            }
            return Redirect("~/PersonalDetails/Edit");
        }

        [HttpGet]
        public ActionResult UploadImages() //renders the image page where user can upload images
        {
            ViewBag.Action = "create"; //action is used in view to post form in the correct action depending on user location
            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];
            return PartialView("EditPhotosPage", PersonalDetailsHelper.GetUserImages());
        }

        [HttpPost]
        public ActionResult UploadImages(string reloadAction, HttpPostedFileBase file) //receives the uploaded images
        {
            var result = PersonalDetailsHelper.SaveImages(file); //validate and save images
            if (!result.Success)
            {
                TempData["error"] = result.Message;
            }
            else
            {
                TempData["success"] = result.Message;
            }

            return Redirect("~/PersonalDetails/"+ reloadAction); //return to the corresponding action based on location (register or edit from inside the application)
        }

        [HttpPost]
        public ActionResult DeleteImage(int id) //action that deletes a user image, id is record id from db not image id/name
        {
            var result = PersonalDetailsHelper.DeleteImage(id); //validate and delete
            if (result.Success)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return Json(result); //returns to ajax call
        }

        [HttpGet]
        public ActionResult EditPhotos() //renders the image page from inside the application where user can upload - delete images
        {
            ViewBag.Action = "edit"; //action is used in view to post form in the correct action depending on user location
            ViewBag.Error = TempData["error"];
            ViewBag.Success = TempData["success"];
            return PartialView("EditPhotosPage", PersonalDetailsHelper.GetUserImages());
        }

        [HttpPost]
        public ActionResult SetPhotoAsProfile(int id) //action that sets a user image as profile picture, id is record id from db not image id/name
        {
            var result = PersonalDetailsHelper.SetPhotoAsProfile(id); //validate and update 
            if (result.Success)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["error"] = result.Message;
            }

            return Json(result); //returns to ajax call
        }
    }
}