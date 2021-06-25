using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace DatingApplication.Helpers
{
    public static class PasswordHelper
    {
        public static OperationResult RegisterMatch(string password1, string password2) //validate register password input
        {
            if(password1 != password2)
            {
                return new OperationResult { Success = false, Message = "Οι κωδικοί πρόσβασης δεν ταιριάζουν" };
            }
            return new OperationResult();

        }

        public static string Encrypt(string password) //encrypt the input password using SHA256
        {
            var sha = SHA256Managed.Create();
            var encoding = new UTF8Encoding();
            var hash = sha.ComputeHash(encoding.GetBytes(password));
            string shash = "";
            hash.ForEach(b =>  //the byte hash is converted to string for easier management
            {
                shash += b.ToString();
            });
            return shash;
        }
        
    }
}