using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Models
{
    public class PersonalDetailsViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<SelectListItem> Genders { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<SelectListItem> HairColors { get; set; }
        public string HairColor { get; set; }
        public List<SelectListItem> EyeColors { get; set; }
        public string EyeColor { get; set; }
        public List<hobbies> Hobbies { get; set; }
        public List<hobbies> SelectedHobbies { get; set; }
        public PostedHobbies PostedHobbies { get; set; }
        public string SelfDescription { get; set; }
    }

    public class PostedHobbies
    {
        public string[] HobbyIds { get; set; }
    }
}