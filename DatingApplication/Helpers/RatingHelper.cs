using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class RatingHelper
    {
        public static int GetUserRating(int id) //retrieves the rating the logged in user gave to the user with the id in the parameter
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;

            using(var db = new DatingEntities())
            {
                return db.ratings.Where(r => r.user_rating == userId && r.user_rated == id).Select(r => r.rating).FirstOrDefault();
            }
        }
    
        public static OperationResult InsertRating(int id, int rating) //record and save the rating, the id is the user being rated
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;

            try
            {
                using(var db = new DatingEntities())
                {
                    db.ratings.Add(new ratings { user_rating = userId, user_rated = id, rating = rating, rating_date = DateTime.Now });
                    db.SaveChanges();
                    return new OperationResult { Success = true, Message = "Η βαθμολογία αποθηκεύτηκε με επιτυχία." };
                }

            }
            catch(Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά τη βαθμολόγηση." };
            }
        }

        public static decimal GetRatingAverage() //retrieves the average rating of the logged in user in the past 7 days
        {
            using (var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                int sum;
                var weekDate = DateTime.Now.AddDays(-7); //go back 7 days
                var ratings = db.ratings.Where(r => r.user_rated == userId && r.rating_date >= weekDate).ToList();

                if (ratings.Count() == 0) //if no ratings, average is 0
                {
                    return 0;
                }
                else //compute average
                {
                    sum = ratings.Select(r => r.rating).Sum();
                    decimal avg = (decimal)sum / (decimal)ratings.Count();
                    return avg;
                }
            }
        }
    }
}