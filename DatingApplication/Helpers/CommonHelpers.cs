using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class CommonHelpers
    {
        public static BasicUserInfo GetLoggedUserInfo() //retrieve the basic info of the logged in user, held in session
        {
            return HttpContext.Current.Session["user"] as BasicUserInfo;
        }

        public static void MarkUserAsLogedIn(string email) //mark user as login by inserting an object of his basic info in a session variable
        {
            using(var db = new DatingEntities())
            {
                var userId = db.users.Where(u => u.email == email).Select(u => u.id).FirstOrDefault();
                
                //get basiv data
                var data = db.user_details.Where(u => u.user_id == userId).Select(u => new { Name = u.name, Surname = u.surname }).FirstOrDefault();

                var weekDate = DateTime.Now.AddDays(-7); //previous week date, used to get the profile visits of the week

                var user = new BasicUserInfo { 
                    Id = userId,
                    Name = data?.Name,
                    Surname = data?.Surname,
                    Category = db.users.Where(u => u.id == userId).Select(u => u.category).FirstOrDefault(),
                    ProfileVisits = db.profile_visits.Where(u => u.user_visited == userId && u.visit_date >= weekDate).Count()
                };

                HttpContext.Current.Session["user"] = user;
            }
        }

        public static void UpdateUserInfo(int Id) //updates the user logged in user info, used during registration since not all the info we want is available from the start
        {
            using (var db = new DatingEntities())
            {

                var data = db.user_details.Where(u => u.user_id == Id).Select(u => new { Name = u.name, Surname = u.surname }).FirstOrDefault();

                var weekDate = DateTime.Now.AddDays(-7); //previous week date, used to get the profile visits of the week

                var user = new BasicUserInfo
                {
                    Id = Id,
                    Name = data?.Name,
                    Surname = data?.Surname,
                    Category = db.users.Where(u => u.id == Id).Select(u => u.category).FirstOrDefault(),
                    ProfileVisits = db.profile_visits.Where(u => u.user_visited == Id && u.visit_date >= weekDate).Count()
                };

                ClearSession();
                HttpContext.Current.Session["user"] = user;
            }
        }

        public static string GetUserProfileImage() //gets the fileaname from the db of the image that is used as profile image
        {
            var userId = GetLoggedUserInfo().Id;
            using(var db = new DatingEntities())
            {
                return db.user_images.Where(u => u.user_id == userId && u.profile_image == 1).Select(u => u.image_path).FirstOrDefault();
            }
        }

        public static void ClearSession() //clear logged in used info
        {
            HttpContext.Current.Session["user"] = null;
        }
    }
}