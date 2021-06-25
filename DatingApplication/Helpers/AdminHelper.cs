using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class AdminHelper
    {
        //retrieves and calculates all the stats tha are displayed in the admin dashboard
        public static AdminStatsViewModel GetAdminStats()
        {
            var stats = new AdminStatsViewModel();
            
            using(var db = new DatingEntities())
            {
                var allUsers = db.users.Where(u => u.category != 3).ToList(); //get all users except admins

                var weekDate = DateTime.Now.AddDays(-7);

                var premiumUsersCount = allUsers.Where(u => u.category == 2).ToList().Count();
                var simpleUsersCount = allUsers.Where(u => u.category == 1).ToList().Count();

                stats.NoOfNewUsers = allUsers.Where(u => u.register_date >= weekDate).Count();
                stats.PremiumUsersPerc = Math.Round(((decimal)premiumUsersCount / (decimal)allUsers.Count()) * 100, 0);
                stats.SimpleUsersPerc = Math.Round(((decimal)simpleUsersCount / (decimal)allUsers.Count()) * 100, 0);

                var allUserDetails = db.user_details.ToList();

                stats.WomenPerc = Math.Round(((decimal)allUserDetails.Where(u => u.gender == 2).ToList().Count() / (decimal)allUserDetails.Count()) * 100,0);
                stats.MenPerc = Math.Round(((decimal)allUserDetails.Where(u => u.gender == 1).ToList().Count() / (decimal)allUserDetails.Count()) * 100,0);
                stats.AverageAge = allUserDetails.Select(u => u.age).Sum() / allUserDetails.Count();

                stats.VisitsLastWeek = db.profile_visits.Where(u => u.visit_date >= weekDate).Count();
                stats.RatingsLastWeek = db.ratings.Where(u => u.rating_date >= weekDate).Count();

                var tempHobbies = db.user_hobbies.GroupBy(u => u.hobby_id).Select(u => new { u.Key, count = u.Count() }).OrderByDescending(u => u.count).ToList();
                var top10Hobbies = tempHobbies.Count() >= 10 ? tempHobbies.Select(t => t.Key).ToList().Take(10).ToList() : tempHobbies.Select(t => t.Key).ToList();

                stats.PopularHobbies = db.hobbies.Where(h => top10Hobbies.Contains(h.id)).Select(h => h.description).ToList();
            }

            return stats;

        }
    }
}