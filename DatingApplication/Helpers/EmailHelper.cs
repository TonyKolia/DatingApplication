using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;


namespace DatingApplication.Helpers
{
    public static class EmailHelper
    {
        public static OperationResult IsValid(string email) //validate email using EmailAddressAttribute's validation
        {
            var e = new EmailAddressAttribute();
            return  !e.IsValid(email) ?  new OperationResult { Success = false, Message = "Η μορφή της διεύθυνσης email δεν είναι έγκυρη" } : new OperationResult();
        }

        public static bool Exists(string email) //check if email exists
        {
            using(var db = new DatingEntities())
            {
                return db.users.Any(u => u.email == email);
            }
        }
    }
}