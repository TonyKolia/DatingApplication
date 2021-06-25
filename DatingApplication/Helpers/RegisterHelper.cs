using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatingApplication.Models;

namespace DatingApplication.Helpers
{
    public static class RegisterHelper
    {
        public static OperationResult RegisterNewUser(RegisterViewModel registerInformation) //used to validate register input and register the user
        {
            try
            {
                var result = new OperationResult();
                result = EmailHelper.IsValid(registerInformation.email); //validate email
                if (!result.Success)
                {
                    return result;
                }

                if (EmailHelper.Exists(registerInformation.email)) //check if user already exists
                {
                    return new OperationResult { Success = false, Message = "Το email υπάρχει ήδη." };
                }

                result = PasswordHelper.RegisterMatch(registerInformation.password, registerInformation.ConfirmPassword); //validate password input
                if (!result.Success)
                {
                    return result;
                }

                using (var db = new DatingEntities()) //if successful, save new user in db
                {
                    var newUser = new users
                    {
                        email = registerInformation.email,
                        password = PasswordHelper.Encrypt(registerInformation.password), //has the password before saving
                        category = 1,
                        register_date = DateTime.Now
                    };

                    db.users.Add(newUser);
                    db.SaveChanges();
                }

                CommonHelpers.MarkUserAsLogedIn(registerInformation.email); //logs the user in by keeping basic info in the session variable
            }
            catch(Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την εγγραφή. Παρακαλώ προσπαθήστε ξανά." };
            }

            return new OperationResult();
        }
    }
}