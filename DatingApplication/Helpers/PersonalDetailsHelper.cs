using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Antlr.Runtime.Tree;
using DatingApplication.Models;
using System.Data.Entity;
using System.IO;

namespace DatingApplication.Helpers
{
    public static class PersonalDetailsHelper
    {
        //ViewModel drop-down lists initialization
        public static PersonalDetailsViewModel InitLists(PersonalDetailsViewModel model)
        {
            using(var db = new DatingEntities())
            {
                model.Genders = db.genders.Select(g => new SelectListItem { Value = g.id.ToString(), Text = g.description }).ToList();
                model.EyeColors = db.eye_colors.Select(g => new SelectListItem { Value = g.id.ToString(), Text = g.description }).ToList();
                model.HairColors = db.hair_colors.Select(g => new SelectListItem { Value = g.id.ToString(), Text = g.description }).ToList();
                model.Hobbies = db.hobbies.ToList();

                return model;
            }
        }

        //retrieves the personal details of the user from the db
        public static PersonalDetailsViewModel GetPersonalDetails()
        {
            using(var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var userDetails = db.user_details.Where(u => u.user_id == userId).FirstOrDefault();

                var model = new PersonalDetailsViewModel();
                model.Name = userDetails.name;
                model.Surname = userDetails.surname;
                model.Gender = userDetails.gender.ToString();
                model.Age = userDetails.age;
                model.Height = userDetails.height;
                model.Weight = userDetails.weight;
                model.SelfDescription = userDetails.self_description;
                model.EyeColor = userDetails.eye_color.ToString();
                model.HairColor = userDetails.hair_color.ToString();

                model = PersonalDetailsHelper.InitLists(model);
                model.SelectedHobbies = db.hobbies.Where(h => db.user_hobbies.Where(u => u.user_id == userId).Select(u => u.hobby_id).ToList().Contains(h.id)).ToList();

                return model;
            }
        }

        //inserts the personal details of the user in the db
        public static OperationResult InsertPersonalDetails(PersonalDetailsViewModel personalDetails)
        {
            try
            {
                using (var db = new DatingEntities())
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id;
                    var userDetails = new user_details
                    {
                        user_id = userId,
                        name = personalDetails.Name,
                        surname = personalDetails.Surname,
                        gender = int.Parse(personalDetails.Gender),
                        age = personalDetails.Age,
                        height = personalDetails.Height,
                        weight = personalDetails.Weight,
                        self_description = personalDetails.SelfDescription,
                        eye_color = int.Parse(personalDetails.EyeColor),
                        hair_color = int.Parse(personalDetails.HairColor)
                    };

                    db.user_details.Add(userDetails);

                    foreach(var hobby in personalDetails.PostedHobbies.HobbyIds)
                    {
                        db.user_hobbies.Add(new user_hobbies { user_id = userId, hobby_id = int.Parse(hobby) });
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την αποθήκευση, παρακαλώ προσπαθήστε ξανά." };
            }

            return new OperationResult();
        }

        //updates the persanal details of the user in the db
        public static OperationResult UpdatePersonalDetails(PersonalDetailsViewModel personalDetails)
        {
            try
            {
                using (var db = new DatingEntities())
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id;

                    var userPersonalDetails = db.user_details.Where(u => u.user_id == userId).FirstOrDefault();
                    var userHobies = db.user_hobbies.Where(u => u.user_id == userId).ToList();

                    userPersonalDetails.name = personalDetails.Name;
                    userPersonalDetails.surname = personalDetails.Surname;
                    userPersonalDetails.gender = int.Parse(personalDetails.Gender);
                    userPersonalDetails.age = personalDetails.Age;
                    userPersonalDetails.height = personalDetails.Height;
                    userPersonalDetails.weight = personalDetails.Weight;
                    userPersonalDetails.self_description = personalDetails.SelfDescription;
                    userPersonalDetails.eye_color = int.Parse(personalDetails.EyeColor);
                    userPersonalDetails.hair_color = int.Parse(personalDetails.HairColor);

                    db.Entry(userPersonalDetails).State = EntityState.Modified;

                    //since hobbies are many and may change completely, delete insert is used for cleaner management
                    userHobies.ForEach(uh => db.Entry(uh).State = EntityState.Deleted);

                    if(personalDetails.PostedHobbies != null)
                    {
                        foreach (var hobby in personalDetails.PostedHobbies.HobbyIds)
                        {
                            db.user_hobbies.Add(new user_hobbies { user_id = userId, hobby_id = int.Parse(hobby) });
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την αποθήκευση, παρακαλώ προσπαθήστε ξανά." };
            }

            return new OperationResult { Success = true, Message = "Τα στοιχεία ενημερώθηκαν με επιτυχία." };
        }
    
        //saves the uploaded image (the file name is stored in db and the actual image file is stored on the server)
        public static OperationResult SaveImages(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var fileType = Path.GetExtension(file.FileName); 
                if (!new List<string> { ".jpg", ".jpeg", ".png" }.Contains(fileType)) //check for acceptable file types
                {
                    return new OperationResult { Success = false, Message = "Μη αποδεκτός τύπος αρχείου εικόνας." };
                }

                try
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id.ToString();
                    var userImagesFolder = HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/" + userId + "/");
                    Directory.CreateDirectory(userImagesFolder); //crate a directory on the server with the name of the userId (if directory already exists then nothing happens)

                    using(var db = new DatingEntities())
                    {
                        var intUserId = CommonHelpers.GetLoggedUserInfo().Id;
                        int profileImage = !db.user_images.Any(u => u.user_id == intUserId) ? 1 : 0; //the 1st image uploaded by used is automatically marked as profile image

                        var newImageName = Guid.NewGuid() + Path.GetExtension(file.FileName); //the name of the image is a guid to ensure uniqeness + the original file type
                        var newImagePath = userImagesFolder + newImageName; //create full path (directory + image name)
                        file.SaveAs(newImagePath); //save file on server

                        db.user_images.Add(new user_images { user_id = int.Parse(userId), image_path = newImageName, profile_image = profileImage }); //save path in db
                        db.SaveChanges();
                    }
                    
                    return new OperationResult { Success = true, Message = "Το ανέβασμα ολοκληρώθηκε με επιτυχία" };
                }
                catch (Exception ex)
                {
                    return new OperationResult { Success = false, Message = "Σφάλμα κατά το ανέβασμα, παρακαλώ προσπαθήστε ξανά." };
                }
            }
            return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε μια φωτογραφία για ανέβασμα." };
        }

        //deletes an uploaded image from the server and its path from db
        public static OperationResult DeleteImage(int id) //not image id, record id in db
        {
            try
            {
                using(var db = new DatingEntities())
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id;
                    var imageToDelete = db.user_images.Where(u => u.id == id).FirstOrDefault(); //delete the path from db
                    var path = imageToDelete.image_path;

                    db.Entry(imageToDelete).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    File.Delete(HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/" + userId + "/" + path)); //delete the actual image file from the server

                    return new OperationResult { Success = true, Message = "Διαγράφθηκε με επιτυχία." };
                }
               
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά τη διαγραφή." };
            }
        }

        //retrieve the image paths of all user images & indicator if an image is profile image
        public static List<ImagesViewModel> GetUserImages()
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;

            using(var db = new DatingEntities())
            {
                var userImages = new List<ImagesViewModel>();
                var userImagesFolder = HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/" + userId + "/");
                db.user_images.Where(u => u.user_id == userId).ToList().ForEach(img => userImages.Add(new ImagesViewModel { Id = img.id, Path = userId.ToString()+"/"+img.image_path, Profile = img.profile_image }));
                return userImages;
            }
        }
    
        //marks an uploaded image as profile image
        public static OperationResult SetPhotoAsProfile(int id)
        {
            using(var db = new DatingEntities())
            {
                try
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id;
                    var curentProfile = db.user_images.Where(u => u.user_id == userId && u.profile_image == 1).FirstOrDefault();
                    var newProfile = db.user_images.Where(u => u.user_id == userId && u.id == id).FirstOrDefault();

                    if (curentProfile != null) //"unmark" the old profile image
                    {
                        curentProfile.profile_image = 0;
                        db.Entry(curentProfile).State = System.Data.Entity.EntityState.Modified;
                    }


                    newProfile.profile_image = 1; //mark the new one
                    db.Entry(newProfile).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return new OperationResult { Success = false, Message = "Σφάλμα κατά την ενημέρωση της φωτογραφίας προφίλ." };
                }
                return new OperationResult { Success = true, Message = "Η φωτογραφία προφίλ ενημερώθηκε με επιτυχία." };
            }
        }
    }
}