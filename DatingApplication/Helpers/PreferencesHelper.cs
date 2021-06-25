using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using System.Data.Entity;

namespace DatingApplication.Helpers
{
    public static class PreferencesHelper
    {
        //ViewModel drop-down lists initialization
        public static PreferencesViewModel InitLists(PreferencesViewModel model)
        {
            using (var db = new DatingEntities())
            {
                model.Genders = db.genders.ToList();
                model.EyeColors = db.eye_colors.ToList();
                model.HairColors = db.hair_colors.ToList();
                model.Hobbies = db.hobbies.ToList();
                return model;
            }
        }

        //default values set for the ViewModel fields
        public static PreferencesViewModel SetDefaults(PreferencesViewModel preferences)
        {
            preferences.AgeFrom = preferences.AgeFrom ?? 18;
            preferences.AgeTo = preferences.AgeTo ?? 99;
            preferences.HeightFrom = preferences.HeightFrom ?? 100;
            preferences.HeightTo = preferences.HeightTo ?? 230;
            preferences.WeightFrom = preferences.WeightFrom ?? 30;
            preferences.WeightTo = preferences.WeightTo ?? 500;

            return preferences;
        }

        public static OperationResult InsertPreferences(PreferencesViewModel preferences)
        {
            //we check all the posted preferences list to see if we have any values at all and only then we save in db
            //mainly used for fields / lists with no defaults
            try
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                bool performInsert = false;
                using (var db = new DatingEntities())
                {
                    //age
                    if (preferences.AgeFrom.HasValue || preferences.AgeTo.HasValue)
                    {
                        db.user_preferences_age.Add(new user_preferences_age { user_id = userId, age_from = preferences.AgeFrom, age_to = preferences.AgeTo });
                        performInsert = true;
                    }

                    //height
                    if (preferences.HeightFrom.HasValue || preferences.HeightTo.HasValue)
                    {
                        db.user_preferences_height.Add(new user_preferences_height { user_id = userId, height_from = preferences.HeightFrom, height_to = preferences.HeightTo });
                        performInsert = true;
                    }

                    //weight
                    if (preferences.WeightFrom.HasValue || preferences.WeightTo.HasValue)
                    {
                        db.user_preferences_weight.Add(new user_preferences_weight { user_id = userId, weight_from = preferences.WeightFrom, weight_to = preferences.WeightTo });
                        performInsert = true;
                    }

                    if(preferences.PostedPreferences != null)
                    {
                        //gender
                        if(preferences.PostedPreferences.Genders != null)
                        {
                            foreach (var genderId in preferences.PostedPreferences.Genders)
                            {
                                db.user_preferences_gender.Add(new user_preferences_gender { user_id = userId, gender = int.Parse(genderId) });
                                performInsert = true;
                            }
                        }
                        else
                        {
                            return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε τουλάχιστον: 1 φύλο, 1 χρώμα ματιών και 1 χρώμα μαλλιών." };
                        }

                        //hair
                        if(preferences.PostedPreferences.HairColors != null)
                        {
                            foreach (var hairId in preferences.PostedPreferences.HairColors)
                            {
                                db.user_preferences_hair_color.Add(new user_preferences_hair_color { user_id = userId, hair_color = int.Parse(hairId) });
                                performInsert = true;
                            }
                        }
                        else
                        {
                            return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε τουλάχιστον: 1 φύλο, 1 χρώμα ματιών και 1 χρώμα μαλλιών." };
                        }

                        //eyes
                        if(preferences.PostedPreferences.EyeColors != null)
                        {
                            foreach (var eyeId in preferences.PostedPreferences.EyeColors)
                            {
                                db.user_preferences_eye_color.Add(new user_preferences_eye_color { user_id = userId, eye_color = int.Parse(eyeId) });
                                performInsert = true;
                            }
                        }
                        else
                        {
                            return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε τουλάχιστον: 1 φύλο, 1 χρώμα ματιών και 1 χρώμα μαλλιών." };
                        }

                        //hobbies
                        if(preferences.PostedPreferences.HobbyIds != null)
                        {
                            foreach (var hobbyId in preferences.PostedPreferences.HobbyIds)
                            {
                                db.user_preferences_hobbies.Add(new user_preferences_hobbies { user_id = userId, hobby = int.Parse(hobbyId) });
                                performInsert = true;
                            }
                        }
                    }

                    if (performInsert)
                    {
                        db.SaveChanges();
                        return new OperationResult { Success = true, Message = "Οι προτιμήσεις αποθηκεύτηκαν με επιτυχία." };
                    }
                    else
                    {
                        return new OperationResult { Success = false, Message = "Παρακαλώ εισάγετε μερικές προτιμήσεις." };
                    }
                    
                    
                }
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την αποθήκευση των προτιμήσεων." };
            }
        }
    
        //retrieves the logged in user's preferences from the db
        public static PreferencesViewModel GetPreferences()
        {
            var userId = CommonHelpers.GetLoggedUserInfo().Id;
            var model = new PreferencesViewModel();

            using(var db = new DatingEntities())
            {
                var age = db.user_preferences_age.Where(u => u.user_id == userId).FirstOrDefault();
                var height = db.user_preferences_height.Where(u => u.user_id == userId).FirstOrDefault();
                var weight = db.user_preferences_weight.Where(u => u.user_id == userId).FirstOrDefault();
                var genders = db.genders.Where(g => db.user_preferences_gender.Where(u => u.user_id == userId).Select(u => u.gender).ToList().Contains(g.id)).ToList();
                var eyes = db.eye_colors.Where(e => db.user_preferences_eye_color.Where(u => u.user_id == userId).Select(u => u.eye_color).ToList().Contains(e.id)).ToList();
                var hair = db.hair_colors.Where(h => db.user_preferences_hair_color.Where(u => u.user_id == userId).Select(u => u.hair_color).ToList().Contains(h.id)).ToList();
                var hobbies = db.hobbies.Where(h => db.user_preferences_hobbies.Where(u => u.user_id == userId).Select(u => u.hobby).ToList().Contains(h.id)).ToList();

                model.AgeFrom = age?.age_from;
                model.AgeTo = age?.age_to;
                model.HeightFrom = height?.height_from;
                model.HeightTo = height?.height_to;
                model.WeightFrom = weight?.weight_from;
                model.WeightTo = weight?.weight_to;
                model.SelectedGenders = genders;
                model.SelectedEyeColors = eyes;
                model.SelectedHairColors = hair;
                model.SelectedHobbies = hobbies;

                model = InitLists(model);
                model = SetDefaults(model);
                return model;
            }
        }
        
        //updates the saved preferences of the logged in user
        //since some preferences are multi value (gender, eye color etc), delete - insert is used for cleaner management
        public static OperationResult UpdatePreferences(PreferencesViewModel preferences)
        {
            try
            {
                var userId = CommonHelpers.GetLoggedUserInfo().Id;
                bool performDelete = false;
                //delete existing preferences, insert new ones
                using (var db = new DatingEntities())
                {
                    var agePref = db.user_preferences_age.Where(u => u.user_id == userId).FirstOrDefault();
                    if (agePref != null)
                    {
                        db.Entry(agePref).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var weightPref = db.user_preferences_weight.Where(u => u.user_id == userId).FirstOrDefault();
                    if (weightPref != null)
                    {
                        db.Entry(weightPref).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var heightPref = db.user_preferences_height.Where(u => u.user_id == userId).FirstOrDefault();
                    if (heightPref != null)
                    {
                        db.Entry(heightPref).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var genderPref = db.user_preferences_gender.Where(u => u.user_id == userId).ToList();
                    foreach (var g in genderPref)
                    {
                        db.Entry(g).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var eyesPref = db.user_preferences_eye_color.Where(u => u.user_id == userId).ToList();
                    foreach (var e in eyesPref)
                    {
                        db.Entry(e).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var hairPref = db.user_preferences_hair_color.Where(u => u.user_id == userId).ToList();
                    foreach (var h in hairPref)
                    {
                        db.Entry(h).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    var hobbyPref = db.user_preferences_hobbies.Where(u => u.user_id == userId).ToList();
                    foreach (var h in hobbyPref)
                    {
                        db.Entry(h).State = EntityState.Deleted;
                        performDelete = true;
                    }

                    if (performDelete)
                    {
                        db.SaveChanges();
                    }

                    var result = InsertPreferences(preferences); //insert new preferences
                    if (!result.Success)
                    {
                        return result;
                    }

                    return new OperationResult { Success = true, Message = "Οι προτιμήσεις ενημερώθηκαν με επιτυχία." };
                }
            }
            catch(Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την ενημέρωση των προτιμήσεων." };
            }
        }

        //validate the preferences input of the user, this is used in the search since we need non default fields to have a value to bring a result back
        public static OperationResult ValidateInputPreferences(PreferencesViewModel preferences)
        {
            if (preferences.PostedPreferences != null)
            {
                //gender
                if (preferences.PostedPreferences.Genders == null || preferences.PostedPreferences.HairColors == null || preferences.PostedPreferences.EyeColors == null)
                {
                    return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε τουλάχιστον: 1 φύλο, 1 χρώμα ματιών και 1 χρώμα μαλλιών." };
                }
                else
                {
                    return new OperationResult { Success = true };
                }
            }
            else
            {
                return new OperationResult { Success = false, Message = "Παρακαλώ επιλέξτε τουλάχιστον: 1 φύλο, 1 χρώμα ματιών και 1 χρώμα μαλλιών." };
            }
        }
    }
}