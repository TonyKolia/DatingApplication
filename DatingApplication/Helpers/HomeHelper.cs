using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI.WebControls;

namespace DatingApplication.Helpers
{
    public static class HomeHelper
    {
        //all the possibles types of matching based on user input in preferences or search
        enum MatchingType{
            AppearanceOnly,
            HobbiesOnly,
            Full,
            Default
        }

        //calculates the matching type depending on user input in preferences or search
        private static MatchingType GetMatchingType(PreferencesViewModel serachCriteria, bool fromSearch = false)
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            bool hasAppearancePrefs = false;
            bool hasHobbiesPrefs = false;

            //in order to define a matching type we look at the input
            //appearance only data -> appearance matching type
            //hobby only data -> hobby matching type
            //appearance & hobby only data -> full matching type
            //no data -> default matching type

            //if input is from search we need to check different input lists since the data is posted from form and not retrieved from db
            //this is beacause of CheckBoxListFor extension usage, more information in documentation
            if (fromSearch) 
            {
                //check appearance lists and mark if any of them has data
                if(serachCriteria.PostedPreferences.EyeColors != null && serachCriteria.PostedPreferences.Genders != null && serachCriteria.PostedPreferences.HairColors != null)
                {
                    hasAppearancePrefs = true;
                }

                //check hobby list and mark if it has data
                if (serachCriteria.PostedPreferences.HobbyIds != null)
                {
                    hasHobbiesPrefs = true;
                }

                //calculate matching type based on above mention logic
                if (hasAppearancePrefs && hasHobbiesPrefs)
                {
                    return MatchingType.Full;
                }
                else if (hasHobbiesPrefs == true && hasHobbiesPrefs == false)
                {
                    return MatchingType.HobbiesOnly;
                }
                else if (hasHobbiesPrefs == false && hasAppearancePrefs == true)
                {
                    return MatchingType.AppearanceOnly;
                }
                else
                {
                    return MatchingType.Default;
                }
            }
            else
            {
                using (var db = new DatingEntities())
                {
                    //same logic as above but this time we are checking saved data in db, this is for preferences
                    if (db.user_preferences_age.Any(u => u.user_id == userId) || db.user_preferences_weight.Any(u => u.user_id == userId)
                       || db.user_preferences_height.Any(u => u.user_id == userId) || db.user_preferences_hair_color.Any(u => u.user_id == userId)
                       || db.user_preferences_eye_color.Any(u => u.user_id == userId) || db.user_preferences_gender.Any(u => u.user_id == userId))
                    {
                        hasAppearancePrefs = true;
                    }

                    if (db.user_preferences_hobbies.Any(u => u.user_id == userId))
                    {
                        hasHobbiesPrefs = true;
                    }

                    if (hasAppearancePrefs && hasHobbiesPrefs)
                    {
                        return MatchingType.Full;
                    }
                    else if (hasHobbiesPrefs == true && hasHobbiesPrefs == false)
                    {
                        return MatchingType.HobbiesOnly;
                    }
                    else if (hasHobbiesPrefs == false && hasAppearancePrefs == true)
                    {
                        return MatchingType.AppearanceOnly;
                    }
                    else
                    {
                        return MatchingType.Default;
                    }
                }
            }
        }

        public static List<UserInfoViewModel> GetUserMatches(PreferencesViewModel userPrefs = null, bool fromSearch = false)
        {
            using(var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var matchingType = GetMatchingType(userPrefs, fromSearch); //get the matching type
                if(userPrefs == null) //if from search, the preferences wont be null since they are posted as criteria
                {
                    userPrefs = PreferencesHelper.GetPreferences(); //else retrieve them from db (saved preferences)
                }

                var userMatchesId = new List<int>();


                //based on the matching type, differenct methods are needed to retrieve the matching user ids
                switch (matchingType) 
                {
                    case MatchingType.Full:
                        userMatchesId = GetFullMatches(userPrefs, fromSearch: fromSearch);
                        break;

                    case MatchingType.AppearanceOnly:
                        userMatchesId = GetAppearanceMatches(userPrefs, fromSearch: fromSearch);
                        break;

                    case MatchingType.HobbiesOnly:
                        userMatchesId = GetHobbiesMatches(userPrefs, fromSearch: fromSearch);
                        break;

                    case MatchingType.Default:
                        userMatchesId = GetDefaultMatches();
                        break;

                    default:
                        break;
                }

                //once we have the matching user ids, get the matching users info from db
                var model = db.user_details.Where(u => u.user_id != userId && userMatchesId.Contains(u.user_id)).Select(u => new UserInfoViewModel
                {
                    user_id = u.user_id,
                    name = u.name,
                    age = u.age,
                    self_description = u.self_description,
                    ProfileImagePath = db.user_images.Where(i => i.user_id == u.user_id && i.profile_image == 1).Select(i => i.image_path).FirstOrDefault()
                }).ToList();

                return model;
            }
        }

        private static List<int> GetDefaultMatches()
        {
            //if no preferences - criteria, just get the all the users at random
            using(var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var userCateg = CommonHelpers.GetLoggedUserInfo().Category;
                var defaultMatches = db.users.Where(u => u.id != userId).OrderByDescending(u => u.register_date).Select(u => u.id).ToList();

                if(userCateg == 1)
                {
                    if(defaultMatches.Count() > 5)
                        return defaultMatches.Take(5).ToList(); //limit simple user to just 12 results

                }
               
                return defaultMatches;
            }
        }

        private static List<int> GetAppearanceMatches(PreferencesViewModel userPrefs, bool internalUse = false, bool fromSearch = false)
        {
            //if only appearance preferences - criteria get matched based on that criteria only
            //used in full matches as well, see below
            using (var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var userCateg = CommonHelpers.GetLoggedUserInfo().Category;
                var genderId = new List<int>();
                var eyeIds = new List<int>();
                var hairIds = new List<int>();
                //again, if from search slightly different setup due to data being posted instead from db
                if (fromSearch)
                {
                    foreach (var gender in userPrefs.PostedPreferences.Genders)
                    {
                        genderId.Add(int.Parse(gender));
                    }

                    foreach (var eye in userPrefs.PostedPreferences.EyeColors)
                    {
                        eyeIds.Add(int.Parse(eye));
                    }

                    foreach (var hair in userPrefs.PostedPreferences.HairColors)
                    {
                        hairIds.Add(int.Parse(hair));
                    }
                }
                else
                {
                    genderId = userPrefs.SelectedGenders.Select(g => g.id).ToList();
                    eyeIds = userPrefs.SelectedEyeColors.Select(e => e.id).ToList();
                    hairIds = userPrefs.SelectedHairColors.Select(h => h.id).ToList();
                }

                //get all the user ids matching the preferences - criteria data
                var matchesOnAppeareance = db.user_details.Where(u => userPrefs.AgeFrom <= u.age && u.age <= userPrefs.AgeTo
                                   && userPrefs.HeightFrom <= u.height && u.height <= userPrefs.HeightTo
                                   && userPrefs.WeightFrom <= u.weight && u.weight <= userPrefs.WeightTo
                                   && genderId.Contains(u.gender)
                                   && eyeIds.Contains(u.eye_color)
                                   && hairIds.Contains(u.hair_color)
                                   && u.user_id != userId
                                 ).Select(u => u.user_id).ToList();

                if (internalUse) //returns the full result to the full match method
                {
                    return matchesOnAppeareance;
                }

                if (userCateg == 1) //limit simple user to just 12 results
                {
                    if (matchesOnAppeareance.Count() > 5)
                        return matchesOnAppeareance.Take(5).ToList();

                }

                return matchesOnAppeareance;
            }
        }
        
        private static List<int> GetHobbiesMatches(PreferencesViewModel userPrefs, bool internalUse = false, bool fromSearch = false)
        {
            //if only hobby preferences - criteria get matched based on that criteria only
            //used in full matches as well, see below
            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            var userCateg = CommonHelpers.GetLoggedUserInfo().Category;
            var hobbiesId = new List<int>();

            //again, if from search slightly different setup due to data being posted instead from db
            if (fromSearch)
            {
                foreach (var hobby in userPrefs.PostedPreferences.HobbyIds)
                {
                    hobbiesId.Add(int.Parse(hobby));
                }
            }
            else
            {
                hobbiesId = userPrefs.SelectedHobbies.Select(h => h.id).ToList();
            }

            using (var db = new DatingEntities())
            {
                //get user ids mathing on hobbies
                var matchesOnHobbies = db.user_hobbies.Where(u => u.user_id != userId && hobbiesId.Contains(u.hobby_id)).Select(u => u.user_id).Distinct().ToList();

                if (internalUse) //returns the full result to the full match method
                { 
                    return matchesOnHobbies;
                }

                if (userCateg == 1) //limit simple user to just 12 results
                {
                    if (matchesOnHobbies.Count() > 5)
                        return matchesOnHobbies.Take(5).ToList();

                }

                return matchesOnHobbies;
            }
        }

        private static List<int> GetFullMatches(PreferencesViewModel userPrefs, bool fromSearch = false)
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            var userCateg = CommonHelpers.GetLoggedUserInfo().Category;
            var appearanceMatches = GetAppearanceMatches(userPrefs, internalUse: true, fromSearch: fromSearch); //get appearance matches
            var hobbiesMatches = GetHobbiesMatches(userPrefs, internalUse: true, fromSearch: fromSearch); //get hobby matches
            var fullMatches = new List<int>();


            //both match types must contain records for a full match
            if(appearanceMatches.Count() == 0 || hobbiesMatches.Count() == 0)
            {
                return fullMatches;
            }

            foreach(var am in appearanceMatches) //find the ids that exist in both lists, meaning that these ids are a match on appearance AND on hobbies
            {
                if (hobbiesMatches.Contains(am))
                {
                    fullMatches.Add(am);
                }
            }

            if(userCateg == 1)
            {
                if(fullMatches.Count() >= 5) //limit simple user to just 12 results 
                {
                    return fullMatches.Take(5).ToList();
                }
            }

            return fullMatches;

        }

        //get the users that visited the profile of the logged in user in the last 7 days
        public static List<UserInfoViewModel> GetProfilesVistedUsers()
        {
            using(var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var weekDate = DateTime.Now.AddDays(-7); //go back 7 days
                var visitingUsers = db.profile_visits.Where(p => p.user_visited == userId && p.visit_date >= weekDate).Select(p => p.user_visiting).Distinct().ToList();
                var model = db.user_details.Where(u => u.user_id != userId && visitingUsers.Contains(u.user_id)).Select(u => new UserInfoViewModel
                {
                    user_id = u.user_id,
                    name = u.name,
                    age = u.age,
                    self_description = u.self_description,
                    ProfileImagePath = db.user_images.Where(i => i.user_id == u.user_id && i.profile_image == 1).Select(i => i.image_path).FirstOrDefault()
                }).ToList();

                return model;
            }
        }

        //get the users that rated the profile of the logged in user in the last 7 days
        public static List<UserInfoViewModel> GetProfileRaters()
        {
            using (var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var weekDate = DateTime.Now.AddDays(-7); //go back 7 days
                var ratingUsers = db.ratings.Where(p => p.user_rated == userId && p.rating_date >= weekDate).Select(p => p.user_rating).Distinct().ToList();
                var model = db.user_details.Where(u => u.user_id != userId && ratingUsers.Contains(u.user_id)).Select(u => new UserInfoViewModel
                {
                    user_id = u.user_id,
                    name = u.name,
                    age = u.age,
                    self_description = u.self_description,
                    ProfileImagePath = db.user_images.Where(i => i.user_id == u.user_id && i.profile_image == 1).Select(i => i.image_path).FirstOrDefault()
                }).ToList();

                return model;
            }
        }

        //retieves all the users, except the login user
        public static List<UserInfoViewModel> GetAllUsers()
        {
            using (var db = new DatingEntities())
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                var model = db.user_details.Where(u => u.user_id != userId).Select(u => new UserInfoViewModel
                {
                    user_id = u.user_id,
                    name = u.name,
                    age = u.age,
                    self_description = u.self_description,
                    ProfileImagePath = db.user_images.Where(i => i.user_id == u.user_id && i.profile_image == 1).Select(i => i.image_path).FirstOrDefault()
                }).ToList();

                return model;
            }
        }
    }
}