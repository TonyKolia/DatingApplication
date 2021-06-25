using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class LoginHelper
    {
        public static OperationResult LoginUser(users LoginInfo) //used to login the user
        {
            using(var db = new DatingEntities())
            {
                try
                {
                    var result = EmailHelper.IsValid(LoginInfo.email); //validate email
                    if (!result.Success)
                    {
                        return result;
                    }

                    var user = db.users.Where(u => u.email == LoginInfo.email).FirstOrDefault();
                    if (user is null || PasswordHelper.Encrypt(LoginInfo.password) != user.password) //check if user exists and validate password
                    {
                        return new OperationResult { Success = false, Message = "Τα στοιχεία σύνδεσης δεν είναι έγκυρα." };
                    }
                }
                catch (Exception ex)
                {

                    return new OperationResult { Success = false, Message = "Σφάλμα κατά τη σύνδεση, παρακαλώ προσπαθήστε ξανά." };
                }
            }

            CommonHelpers.MarkUserAsLogedIn(LoginInfo.email);

            return new OperationResult();
        }
    }
}