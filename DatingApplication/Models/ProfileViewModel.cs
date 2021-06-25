using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string SelfDescription { get; set; }
        public string EyeColor { get; set; }
        public string HairColor { get; set; }
        public List<string> Hobbies { get; set; }
        public string UserCategory { get; set; }
        public List<string> ImagePaths { get; set; }
    }
}