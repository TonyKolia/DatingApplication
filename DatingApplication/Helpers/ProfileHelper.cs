using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class ProfileHelper
    {
        public  static ProfileViewModel GetProfileData(int id) //retrieves all the data that make up the ProfileViewModel, shown in user profile
        {
            using(var db = new DatingEntities())
            {
                var userDetails = db.user_details.Where(u => u.user_id == id).FirstOrDefault();
                var userHobbies = db.user_hobbies.Where(u => u.user_id == id).Select(u => u.hobby_id).ToList();

                var profileData = new ProfileViewModel
                {
                    Id = id.ToString(),
                    Name = userDetails.name,
                    Surname = userDetails.surname,
                    Age = userDetails.age,
                    Gender = db.genders.Where(g => g.id == userDetails.gender).Select(g => g.description).FirstOrDefault(),
                    Height = userDetails.height.ToString(),
                    Weight = userDetails.weight.ToString(),
                    SelfDescription = userDetails.self_description,
                    EyeColor = db.eye_colors.Where(g => g.id == userDetails.eye_color).Select(g => g.description).FirstOrDefault(),
                    HairColor = db.hair_colors.Where(g => g.id == userDetails.hair_color).Select(g => g.description).FirstOrDefault(),
                    Hobbies = db.hobbies.Where(h => db.user_hobbies.Where(uh => uh.user_id == id).Select(uh => uh.hobby_id).ToList().Contains(h.id)).Select(h => h.description).ToList(),
                    UserCategory = db.user_categories.Where(u => u.id == db.users.Where(uu => uu.id == id).Select(uu => uu.category).FirstOrDefault()).Select(u => u.description).FirstOrDefault(),
                    ImagePaths = db.user_images.Where(u => u.user_id == id).OrderByDescending(u => u.profile_image).Select(u => u.image_path).ToList()
                };

                return profileData;
            }

        }
    
        public static void MarkProfileVisit(int id) //records and saves the profile visit, id is the visited user
        {
            using(var db = new DatingEntities())
            {
                db.profile_visits.Add(new profile_visits
                {
                    user_visiting = CommonHelpers.GetLoggedUserInfo().Id,
                    user_visited = id,
                    visit_date = DateTime.Now
                });

                db.SaveChanges();
            }
        }
    }
}