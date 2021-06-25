using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Models
{
    public class RegisterViewModel : users
    {
        public string ConfirmPassword { get; set; }
    }
}